// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.Azure.IIoT.Diagnostics {
    using System;
    using Microsoft.Extensions.Logging;
    using IAspLogger = Extensions.Logging.ILogger;

    /// <summary>
    /// Logger implementation
    /// </summary>
    public class ExtensionLogger : BaseLogger {

        /// <summary>
        /// Create logger
        /// </summary>
        /// <param name="logger"></param>
        public ExtensionLogger(IAspLogger logger) :
            this(logger, null) {
        }

        /// <summary>
        /// Create logger
        /// </summary>
        /// <param name="config"></param>
        /// <param name="logger"></param>
        public ExtensionLogger(IAspLogger logger, ILogConfig config) :
            base(config?.ProcessId) {
            _logger = logger ??
                throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Log debug
        /// </summary>
        /// <param name="message"></param>
        protected override sealed void Debug(Func<string> message) =>
            _logger.LogDebug(message());

        /// <summary>
        /// Log info
        /// </summary>
        /// <param name="message"></param>
        protected override sealed void Info(Func<string> message) =>
            _logger.LogInformation(message());

        /// <summary>
        /// Log warning
        /// </summary>
        /// <param name="message"></param>
        protected override sealed void Warn(Func<string> message) =>
            _logger.LogWarning(message());

        /// <summary>
        /// Log error
        /// </summary>
        /// <param name="message"></param>
        protected override sealed void Error(Func<string> message) =>
            _logger.LogError(message());

        private readonly IAspLogger _logger;
    }
}
