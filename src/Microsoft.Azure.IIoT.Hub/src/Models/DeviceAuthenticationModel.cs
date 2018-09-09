// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.Azure.IIoT.Hub.Models {
    using Newtonsoft.Json;

    /// <summary>
    /// Authentication information
    /// </summary>
    public class DeviceAuthenticationModel {

        /// <summary>
        /// Primary sas key
        /// </summary>
        [JsonProperty(PropertyName = "primaryKey",
            NullValueHandling = NullValueHandling.Ignore)]
        public string PrimaryKey { get; set; }

        /// <summary>
        /// Secondary sas key
        /// </summary>
        [JsonProperty(PropertyName = "secondaryKey",
            NullValueHandling = NullValueHandling.Ignore)]
        public string SecondaryKey { get; set; }
    }
}
