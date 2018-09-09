// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.Azure.IIoT.Utils {
    using Microsoft.Azure.IIoT.Diagnostics;
    using Microsoft.Azure.IIoT.Exceptions;
    using System;
    using System.Threading.Tasks;
    using System.Threading;

    /// <summary>
    /// Retry helper class with different retry policies
    /// </summary>
    public static class Retry {

        private static readonly object Semaphore = new object();

        /// <summary>Retry count max</summary>
        public static int MaxRetryCount = 10;

        /// <summary>
        /// Default exponential policy with 20% jitter
        /// </summary>
        public static Func<int, Exception, int> Exponential => (k, ex) => {
            if (k > ExponentialMaxRetryCount) {
                k = ExponentialMaxRetryCount;
            }
            var backoff = r.Next(
                (int)(ExponentialBackoffIncrement * 0.8),
                (int)(ExponentialBackoffIncrement * 1.2));
            var exp = 0.5 * (Math.Pow(2, k) - 1);
            var result = (int)(exp * backoff);
            System.Diagnostics.Debug.Assert(result > 0);
            return result;
        };
        private static Random r = new Random();
        /// <summary>Max retry count for exponential policy</summary>
        public static int ExponentialMaxRetryCount = 13;
        /// <summary>Exponential backoff increment</summary>
        public static int ExponentialBackoffIncrement = 10;

        /// <summary>
        /// Default linear policy
        /// </summary>
        public static Func<int, Exception, int> Linear => (k, ex) =>
            Math.Min(k, LinearMaxRetryDelayCount) * LinearBackoffDelta;
        /// <summary>Max retry multiplier</summary>
        public static int LinearMaxRetryDelayCount = 20;
        /// <summary>Incremental delay</summary>
        public static int LinearBackoffDelta = 1000;

        /// <summary>
        /// No backoff - just wait backoff delta
        /// </summary>
        public static Func<int, Exception, int> NoBackoff => (k, ex) => NoBackoffDelta;
        /// <summary>Time between retry</summary>
        public static int NoBackoffDelta = 1000;

        /// <summary>
        /// Retries a piece of work
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="ct"></param>
        /// <param name="work"></param>
        /// <param name="cont"></param>
        /// <param name="policy"></param>
        /// <param name="maxRetry"></param>
        /// <returns></returns>
        public static async Task Do(ILogger logger, CancellationToken ct, Func<Task> work,
            Func<Exception, bool> cont, Func<int, Exception, int> policy, int maxRetry) {
            for (var k = 1; ; k++) {
                if (ct.IsCancellationRequested) {
                    throw new TaskCanceledException();
                }
                try {
                    await work();
                    return;
                }
                catch (Exception ex) {
                    await DelayOrThrow(logger, cont, policy, maxRetry, k, ex, ct);
                }
            }
        }

        /// <summary>
        /// Retries a piece of work with return type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="logger"></param>
        /// <param name="ct"></param>
        /// <param name="work"></param>
        /// <param name="cont"></param>
        /// <param name="policy"></param>
        /// <param name="maxRetry"></param>
        /// <returns></returns>
        public static async Task<T> Do<T>(ILogger logger, CancellationToken ct, Func<Task<T>> work,
            Func<Exception, bool> cont, Func<int, Exception, int> policy, int maxRetry) {
            for (var k = 1; ; k++) {
                if (ct.IsCancellationRequested) {
                    throw new TaskCanceledException();
                }
                try {
                    return await work();
                }
                catch (Exception ex) {
                    await DelayOrThrow(logger, cont, policy, maxRetry, k, ex, ct);
                }
            }
        }

        /// <summary>
        /// Retries a piece of work
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="ct"></param>
        /// <param name="work"></param>
        /// <param name="cont"></param>
        /// <param name="policy"></param>
        /// <param name="maxRetry"></param>
        /// <returns></returns>
        public static async Task Do(ILogger logger, CancellationToken ct, Action work,
            Func<Exception, bool> cont, Func<int, Exception, int> policy, int maxRetry) {
            for (var k = 1; ; k++) {
                if (ct.IsCancellationRequested) {
                    throw new TaskCanceledException();
                }
                try {
                    work();
                    return;
                }
                catch (Exception ex) {
                    await DelayOrThrow(logger, cont, policy, maxRetry, k, ex, ct);
                }
            }
        }

        /// <summary>
        /// Retries a piece of work with return type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="logger"></param>
        /// <param name="ct"></param>
        /// <param name="work"></param>
        /// <param name="cont"></param>
        /// <param name="policy"></param>
        /// <param name="maxRetry"></param>
        /// <returns></returns>
        public static async Task<T> Do<T>(ILogger logger, CancellationToken ct, Func<T> work,
            Func<Exception, bool> cont, Func<int, Exception, int> policy, int maxRetry) {
            for (var k = 1; ; k++) {
                if (ct.IsCancellationRequested) {
                    throw new TaskCanceledException();
                }
                try {
                    return work();
                }
                catch (Exception ex) {
                    await DelayOrThrow(logger, cont, policy, maxRetry, k, ex, ct);
                }
            }
        }

        /// <summary>
        /// Retry with linear backoff
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="ct"></param>
        /// <param name="work"></param>
        /// <param name="cont"></param>
        /// <returns></returns>
        public static Task WithLinearBackoff(ILogger logger, CancellationToken ct,
            Func<Task> work, Func<Exception, bool> cont) =>
                Do(logger, ct, work, cont, Linear, MaxRetryCount);

        /// <summary>
        /// Retry with linear backoff
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="ct"></param>
        /// <param name="work"></param>
        /// <returns></returns>
        public static Task WithLinearBackoff(ILogger logger, CancellationToken ct,
            Func<Task> work) =>
            WithLinearBackoff(logger, ct, work, ex => ex is ITransientException);

        /// <summary>
        /// Retry with linear backoff
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="work"></param>
        /// <returns></returns>
        public static Task WithLinearBackoff(ILogger logger, Func<Task> work) =>
            WithLinearBackoff(logger, CancellationToken.None, work);

        /// <summary>
        /// Retry with linear backoff
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="cont"></param>
        /// <param name="work"></param>
        /// <returns></returns>
        public static Task WithLinearBackoff(ILogger logger,
            Func<Task> work, Func<Exception, bool> cont) =>
            WithLinearBackoff(logger, CancellationToken.None, work, cont);

        /// <summary>
        /// Retry with linear backoff
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="logger"></param>
        /// <param name="ct"></param>
        /// <param name="work"></param>
        /// <param name="cont"></param>
        /// <returns></returns>
        public static Task<T> WithLinearBackoff<T>(ILogger logger, CancellationToken ct,
            Func<Task<T>> work, Func<Exception, bool> cont) =>
            Do(logger, ct, work, cont, Linear, MaxRetryCount);

        /// <summary>
        /// Retry with linear backoff
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="logger"></param>
        /// <param name="ct"></param>
        /// <param name="work"></param>
        /// <returns></returns>
        public static Task<T> WithLinearBackoff<T>(ILogger logger, CancellationToken ct,
            Func<Task<T>> work) =>
            WithLinearBackoff(logger, ct, work, (ex) => ex is ITransientException);

        /// <summary>
        /// Retry with linear backoff
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="logger"></param>
        /// <param name="work"></param>
        /// <returns></returns>
        public static Task<T> WithLinearBackoff<T>(ILogger logger, Func<Task<T>> work) =>
            WithLinearBackoff(logger, CancellationToken.None, work);

        /// <summary>
        /// Retry with linear backoff
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="logger"></param>
        /// <param name="cont"></param>
        /// <param name="work"></param>
        /// <returns></returns>
        public static Task<T> WithLinearBackoff<T>(ILogger logger,
            Func<Task<T>> work, Func<Exception, bool> cont) =>
            WithLinearBackoff(logger, CancellationToken.None, work, cont);

        /// <summary>
        /// Retry with exponential backoff
        /// </summary>
        /// <param name="ct"></param>
        /// <param name="logger"></param>
        /// <param name="work"></param>
        /// <param name="cont"></param>
        /// <returns></returns>
        public static Task WithExponentialBackoff(ILogger logger, CancellationToken ct,
            Func<Task> work, Func<Exception, bool> cont) =>
             Do(logger, ct, work, cont, Exponential, MaxRetryCount);

        /// <summary>
        /// Retry with exponential backoff
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="ct"></param>
        /// <param name="work"></param>
        /// <returns></returns>
        public static Task WithExponentialBackoff(ILogger logger, CancellationToken ct,
            Func<Task> work) =>
            WithExponentialBackoff(logger, ct, work, ex => ex is ITransientException);

        /// <summary>
        /// Retry with exponential backoff
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="work"></param>
        /// <returns></returns>
        public static Task WithExponentialBackoff(ILogger logger, Func<Task> work) =>
            WithExponentialBackoff(logger, CancellationToken.None, work);

        /// <summary>
        /// Retry with exponential backoff
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="cont"></param>
        /// <param name="work"></param>
        /// <returns></returns>
        public static Task WithExponentialBackoff(ILogger logger,
            Func<Task> work, Func<Exception, bool> cont) =>
            WithExponentialBackoff(logger, CancellationToken.None, work, cont);

        /// <summary>
        /// Retry with exponential backoff
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="logger"></param>
        /// <param name="ct"></param>
        /// <param name="work"></param>
        /// <param name="cont"></param>
        /// <returns></returns>
        public static Task<T> WithExponentialBackoff<T>(ILogger logger, CancellationToken ct,
            Func<Task<T>> work, Func<Exception, bool> cont) =>
            Do(logger, ct, work, cont, Exponential, MaxRetryCount);

        /// <summary>
        /// Retry with exponential backoff
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="logger"></param>
        /// <param name="ct"></param>
        /// <param name="work"></param>
        /// <returns></returns>
        public static Task<T> WithExponentialBackoff<T>(ILogger logger, CancellationToken ct,
            Func<Task<T>> work) =>
            WithExponentialBackoff(logger, ct, work, (ex) => ex is ITransientException);

        /// <summary>
        /// Retry with exponential backoff
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="logger"></param>
        /// <param name="work"></param>
        /// <returns></returns>
        public static Task<T> WithExponentialBackoff<T>(ILogger logger, Func<Task<T>> work) =>
             WithExponentialBackoff(logger, CancellationToken.None, work);

        /// <summary>
        /// Retry with exponential backoff
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="logger"></param>
        /// <param name="cont"></param>
        /// <param name="work"></param>
        /// <returns></returns>
        public static Task<T> WithExponentialBackoff<T>(ILogger logger,
            Func<Task<T>> work, Func<Exception, bool> cont) =>
            WithExponentialBackoff(logger, CancellationToken.None, work, cont);

        /// <summary>
        /// Retry with linear backoff
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="ct"></param>
        /// <param name="work"></param>
        /// <param name="cont"></param>
        /// <returns></returns>
        public static Task WithLinearBackoff(ILogger logger, CancellationToken ct,
            Action work, Func<Exception, bool> cont) =>
                Do(logger, ct, work, cont, Linear, MaxRetryCount);

        /// <summary>
        /// Retry with linear backoff
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="ct"></param>
        /// <param name="work"></param>
        /// <returns></returns>
        public static Task WithLinearBackoff(ILogger logger, CancellationToken ct,
            Action work) =>
            WithLinearBackoff(logger, ct, work, ex => ex is ITransientException);

        /// <summary>
        /// Retry with linear backoff
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="work"></param>
        /// <returns></returns>
        public static Task WithLinearBackoff(ILogger logger, Action work) =>
            WithLinearBackoff(logger, CancellationToken.None, work);

        /// <summary>
        /// Retry with linear backoff
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="cont"></param>
        /// <param name="work"></param>
        /// <returns></returns>
        public static Task WithLinearBackoff(ILogger logger,
            Action work, Func<Exception, bool> cont) =>
            WithLinearBackoff(logger, CancellationToken.None, work, cont);

        /// <summary>
        /// Retry with linear backoff
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="logger"></param>
        /// <param name="ct"></param>
        /// <param name="work"></param>
        /// <param name="cont"></param>
        /// <returns></returns>
        public static Task<T> WithLinearBackoff<T>(ILogger logger, CancellationToken ct,
            Func<T> work, Func<Exception, bool> cont) =>
            Do(logger, ct, work, cont, Linear, MaxRetryCount);

        /// <summary>
        /// Retry with linear backoff
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="logger"></param>
        /// <param name="ct"></param>
        /// <param name="work"></param>
        /// <returns></returns>
        public static Task<T> WithLinearBackoff<T>(ILogger logger, CancellationToken ct,
            Func<T> work) =>
            WithLinearBackoff(logger, ct, work, (ex) => ex is ITransientException);

        /// <summary>
        /// Retry with linear backoff
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="logger"></param>
        /// <param name="work"></param>
        /// <returns></returns>
        public static Task<T> WithLinearBackoff<T>(ILogger logger, Func<T> work) =>
            WithLinearBackoff(logger, CancellationToken.None, work);

        /// <summary>
        /// Retry with linear backoff
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="logger"></param>
        /// <param name="cont"></param>
        /// <param name="work"></param>
        /// <returns></returns>
        public static Task<T> WithLinearBackoff<T>(ILogger logger,
            Func<T> work, Func<Exception, bool> cont) =>
            WithLinearBackoff(logger, CancellationToken.None, work, cont);

        /// <summary>
        /// Retry with exponential backoff
        /// </summary>
        /// <param name="ct"></param>
        /// <param name="logger"></param>
        /// <param name="work"></param>
        /// <param name="cont"></param>
        /// <returns></returns>
        public static Task WithExponentialBackoff(ILogger logger, CancellationToken ct,
            Action work, Func<Exception, bool> cont) =>
             Do(logger, ct, work, cont, Exponential, MaxRetryCount);

        /// <summary>
        /// Retry with exponential backoff
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="ct"></param>
        /// <param name="work"></param>
        /// <returns></returns>
        public static Task WithExponentialBackoff(ILogger logger, CancellationToken ct,
            Action work) =>
            WithExponentialBackoff(logger, ct, work, ex => ex is ITransientException);

        /// <summary>
        /// Retry with exponential backoff
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="work"></param>
        /// <returns></returns>
        public static Task WithExponentialBackoff(ILogger logger, Action work) =>
            WithExponentialBackoff(logger, CancellationToken.None, work);

        /// <summary>
        /// Retry with exponential backoff
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="cont"></param>
        /// <param name="work"></param>
        /// <returns></returns>
        public static Task WithExponentialBackoff(ILogger logger,
            Action work, Func<Exception, bool> cont) =>
            WithExponentialBackoff(logger, CancellationToken.None, work, cont);

        /// <summary>
        /// Retry with exponential backoff
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="logger"></param>
        /// <param name="ct"></param>
        /// <param name="work"></param>
        /// <param name="cont"></param>
        /// <returns></returns>
        public static Task<T> WithExponentialBackoff<T>(ILogger logger, CancellationToken ct,
            Func<T> work, Func<Exception, bool> cont) =>
            Do(logger, ct, work, cont, Exponential, MaxRetryCount);

        /// <summary>
        /// Retry with exponential backoff
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="logger"></param>
        /// <param name="ct"></param>
        /// <param name="work"></param>
        /// <returns></returns>
        public static Task<T> WithExponentialBackoff<T>(ILogger logger, CancellationToken ct,
            Func<T> work) =>
            WithExponentialBackoff(logger, ct, work, (ex) => ex is ITransientException);

        /// <summary>
        /// Retry with exponential backoff
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="logger"></param>
        /// <param name="work"></param>
        /// <returns></returns>
        public static Task<T> WithExponentialBackoff<T>(ILogger logger, Func<T> work) =>
             WithExponentialBackoff(logger, CancellationToken.None, work);

        /// <summary>
        /// Retry with exponential backoff
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="logger"></param>
        /// <param name="cont"></param>
        /// <param name="work"></param>
        /// <returns></returns>
        public static Task<T> WithExponentialBackoff<T>(ILogger logger,
            Func<T> work, Func<Exception, bool> cont) =>
            WithExponentialBackoff(logger, CancellationToken.None, work, cont);

        /// <summary>
        /// Helper to run the delay policy and output additional information.
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="cont"></param>
        /// <param name="policy"></param>
        /// <param name="maxRetry"></param>
        /// <param name="k"></param>
        /// <param name="ex"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        private static async Task DelayOrThrow(ILogger logger, Func<Exception, bool> cont,
            Func<int, Exception, int> policy, int maxRetry, int k, Exception ex,
            CancellationToken ct) {
            if ((k > maxRetry || !cont(ex)) && !(ex is ITransientException)) {
                logger?.Info($"Give up after {k}", () => ex);
                throw ex;
            }
            logger?.Debug($"Retry {k}..", () => ex);
            var delay = policy(k, ex);
            if (delay != 0) {
                await Task.Delay(delay, ct);
            }
        }
    }
}
