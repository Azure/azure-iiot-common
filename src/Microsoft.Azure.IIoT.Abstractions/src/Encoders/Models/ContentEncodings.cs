// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.Azure.IIoT {

    public static class ContentEncodings {

        public const string MimeTypeJson =
            "application/json";
        public const string MimeTypeMsgPack =
            "application/binary-msgpack";

        public const string MimeTypeUaJson =
            "application/ua+json";
        public const string MimeTypeUaNonReversibleJson =
            "application/ua+json+nr";

        // Reference stack encoders
        public const string MimeTypeUaBinary =
            "application/ua+binary";
        public const string MimeTypeUaXml =
            "application/ua+xml";

        public const string MimeTypeUaJsonReference =
            "application/ua+json+ref";
        public const string MimeTypeUaNonReversibleJsonReference =
            "application/ua+json+ref+nr";
    }
}
