// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.Azure.IIoT.Auth.Azure {
    using Microsoft.Azure.IIoT.Auth.Models;
    using Microsoft.Azure.Services.AppAuthentication;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Authenticate using azure service token provider.  This can be used for where you
    /// would not have a token from a logged on user, e.g. in deamon or service to service
    /// scenarios.
    /// This provider works for development, managed service identity and service principal
    /// scenarios.  It can optionally be configured using a connection string provided as
    /// environment variable or injected configuration.  For more information check out
    /// https://docs.microsoft.com/en-us/azure/key-vault/service-to-service-authentication
    /// </summary>
    public class AppAuthenticationProvider : ITokenProvider {

        /// <summary>
        /// Create auth provider.
        /// </summary>
        public AppAuthenticationProvider() : this(null) {
        }

        /// <summary>
        /// Create auth provider
        /// </summary>
        /// <param name="config"></param>
        public AppAuthenticationProvider(IClientConfig config) {
            _config = config;
            _provider = new AzureServiceTokenProvider(GetConnectionString(),
                _config?.Authority ?? kAuthority);
        }

        /// <summary>
        /// Obtain token using the Application authentication token
        /// provider framework.
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="scopes"></param>
        /// <returns></returns>
        public async Task<TokenResultModel> GetTokenForAsync(string resource,
            IEnumerable<string> scopes) {
            var token = await _provider.GetAccessTokenAsync(resource,
                _config?.ClientSecret ?? _config?.TenantId);
            return TokenResultModelEx.Parse(token);
        }

        /// <inheritdoc/>
        public Task InvalidateAsync(string resource) => Task.CompletedTask;

        /// <summary>
        /// Helper to make connection string
        /// </summary>
        /// <returns></returns>
        private string GetConnectionString() {
            // See if configured in environment variable
            var cs = Environment.GetEnvironmentVariable(
                "AzureServicesAuthConnectionString");
            if (!string.IsNullOrEmpty(cs)) {
                return cs;
            }
            if (string.IsNullOrEmpty(_config?.ClientId)) {
                // Run as dev or current user
                return NoClientIdRunAs();
            }
            // Run as app
            cs = $"RunAs=App;AppId={_config.ClientId}";
            if (!string.IsNullOrEmpty(_config.TenantId)) {
                cs += $";TenantId={_config.TenantId}";
            }
            if (!string.IsNullOrEmpty(_config.ClientSecret)) {
                cs += $";AppKey={_config.ClientSecret}";
            }
            return cs;
        }

        /// <inheritdoc/>
        protected virtual string NoClientIdRunAs() => null;

        private const string kAuthority = "https://login.microsoftonline.com/";
        /// <summary>Configuration for derived class</summary>
        protected readonly IClientConfig _config;
        /// <summary>Token provider for derived class</summary>
        protected readonly AzureServiceTokenProvider _provider;
    }
}
