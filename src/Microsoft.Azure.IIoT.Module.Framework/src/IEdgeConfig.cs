// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.Azure.IIoT.Module.Framework.Client {
    using System;

    /// <summary>
    /// Edge configuration
    /// </summary>
    public interface IEdgeConfig {

        /// <summary>
        /// Edge Hub connection string
        /// </summary>
        string EdgeHubConnectionString { get; }

        /// <summary>
        /// Bypass cert validation with hub
        /// </summary>
        bool BypassCertVerification { get; }

        /// <summary>
        /// Transports to use
        /// </summary>
        TransportOption Transport { get; }
    }
}
