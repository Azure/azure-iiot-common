// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.Azure.IIoT.Hub.Models {
    using Newtonsoft.Json;

    /// <summary>
    /// Capabilities
    /// </summary>
    public class DeviceCapabilitiesModel {

        /// <summary>
        /// iotedge device
        /// </summary>
        [JsonProperty(PropertyName = "iotEdge",
            NullValueHandling = NullValueHandling.Ignore)]
        public bool? IotEdge { get; set; }
    }
}
