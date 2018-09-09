// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.Azure.IIoT.Hub.Models {
    using Microsoft.Azure.Devices;

    /// <summary>
    /// Device model extensions
    /// </summary>
    public static class DeviceModelEx {

        /// <summary>
        /// Convert twin to module
        /// </summary>
        /// <param name="module"></param>
        /// <returns></returns>
        public static DeviceModel ToModel(this Module module) {
            return new DeviceModel {
                Id = module.DeviceId,
                ModuleId = module.Id,
                Authentication = module.Authentication.ToModel(),
                Connected = module.ConnectionState == DeviceConnectionState.Connected,
                Enabled = true,
                Etag = module.ETag
            };
        }

        /// <summary>
        /// Convert twin to module
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        public static DeviceModel ToModel(this Device device) {
            return new DeviceModel {
                Id = device.Id,
                ModuleId = null,
                Authentication = device.Authentication.ToModel(),
                Connected = device.ConnectionState == DeviceConnectionState.Connected,
                Enabled = device.Status == DeviceStatus.Enabled,
                Etag = device.ETag
            };
        }

        /// <summary>
        /// Convert twin to module
        /// </summary>
        /// <param name="auth"></param>
        /// <returns></returns>
        public static DeviceAuthenticationModel ToModel(this AuthenticationMechanism auth) {
            return new DeviceAuthenticationModel {
                PrimaryKey = auth.SymmetricKey.PrimaryKey,
                SecondaryKey = auth.SymmetricKey.SecondaryKey
            };
        }
    }
}
