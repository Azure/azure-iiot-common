// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.Azure.IIoT.Utils {
    using Microsoft.Azure.IIoT.Exceptions;
    using System;
    using System.Collections.Generic;
    using Microsoft.Extensions.Configuration;

    /// <summary>
    /// Configuration base helper class
    /// </summary>
    public abstract class ConfigBase {

        /// <summary>
        /// Configuration
        /// </summary>
        public IConfigurationRoot Configuration { get; }

        /// <summary>
        /// Configuration constructor
        /// </summary>
        /// <param name="configuration"></param>
        protected ConfigBase(IConfigurationRoot configuration) {
            Configuration = configuration;

            if (configuration == null) {
                var builder = new ConfigurationBuilder();
                builder.AddEnvironmentVariables();
                configuration = builder.Build();
            }
        }

        /// <summary>
        /// Read variable and replace environment variable if needed
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        protected string GetString(string key, string defaultValue = "") {
            var value = Configuration.GetValue(key, defaultValue);
            if (string.IsNullOrEmpty(value)) {
                return defaultValue;
            }
            return value.Trim();
        }

        /// <summary>
        /// Read boolean
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        protected bool GetBool(string key, bool defaultValue = false) {
            var value = GetString(key, defaultValue.ToString()).ToLowerInvariant();
            var knownTrue = new HashSet<string> { "true", "t", "yes", "y", "1", "-1" };
            var knownFalse = new HashSet<string> { "false", "f", "no", "n", "0" };
            if (knownTrue.Contains(value)) {
                return true;
            }
            if (knownFalse.Contains(value)) {
                return false;
            }
            return defaultValue;
        }

        /// <summary>
        /// Get time span
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        protected TimeSpan GetDuration(string key, TimeSpan? defaultValue = null) {
            if (!TimeSpan.TryParse(GetString(key), out var result)) {
                if (defaultValue != null) {
                    return (TimeSpan)defaultValue;
                }
                throw new InvalidConfigurationException(
                    $"Unable to load timespan value for '{key}' from configuration.");
            }
            return result;
        }

        /// <summary>
        /// Read int
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        protected int GetInt(string key, int defaultValue = 0) {
            try {
                return Convert.ToInt32(GetString(key, defaultValue.ToString()));
            }
            catch (Exception e) {
                throw new InvalidConfigurationException(
                    $"Unable to load configuration value for '{key}'", e);
            }
        }
    }
}
