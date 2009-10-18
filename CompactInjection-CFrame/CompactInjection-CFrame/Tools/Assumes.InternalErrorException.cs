// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
//Code extracted from: MEF project http://mef.codeplex.com/
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics;
using System.Globalization;

namespace CompactInjection.Tools
{
    partial class Assumes
    {
        // The exception that is thrown when an internal assumption failed.
        [Serializable]
        [SuppressMessage("Microsoft.Design", "CA1064:ExceptionsShouldBePublic")]
        private class InternalErrorException : Exception
        {
            public InternalErrorException(string message)
                : base(string.Format(CultureInfo.CurrentCulture, "Internal error occurred. Additional information: '{0}'.", message))
            {
            }

//#if !SILVERLIGHT
//            [System.Security.SecurityCritical, System.Security.SecurityTreatAsSafe]
//            protected InternalErrorException(SerializationInfo info, StreamingContext context)
//                : base(info, context)
//            {
//            }
//#endif
        }
    }
}
