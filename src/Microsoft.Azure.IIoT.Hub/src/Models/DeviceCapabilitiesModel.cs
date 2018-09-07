// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.Azure.IIoT.Hub.Models {
    using Newtonsoft.Json;

    public class DeviceCapabilitiesModel {

        /// <summary>
        /// iotedge device
        /// </summary>
        [JsonProperty(PropertyName = "iotedge",
            NullValueHandling = NullValueHandling.Ignore)]
        public bool? iotedge { get; set; }
    }
}
