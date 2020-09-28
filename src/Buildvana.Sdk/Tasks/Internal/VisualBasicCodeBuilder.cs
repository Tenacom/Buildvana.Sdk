// -----------------------------------------------------------------------------------
// Copyright (C) Riccardo De Agostini and Buildvana contributors. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.
//
// Part of this file may be third-party code, distributed under a compatible license.
// See THIRD-PARTY-NOTICES file in the project root for third-party copyright notices.
// -----------------------------------------------------------------------------------

namespace Buildvana.Sdk.Tasks.Internal
{
#pragma warning disable CA1812 // Avoid uninstantiated internal classes - This class is used indirectly via generics
    internal sealed class VisualBasicCodeBuilder : CodeBuilder
#pragma warning restore CA1812
    {
        protected override string LineCommentPrefix => "// ";

        public override void QuotedText(string str)
        {
            Text('"');
            var length = str.Length;
            if (length > 0)
            {
                var position = 0;
                while (position < length)
                {
                    var nextPosition = str.IndexOf('"', position);
                    if (nextPosition < 0)
                    {
                        Text(str, position, length - position);
                        position = length;
                    }
                    else if (nextPosition == position)
                    {
                        Text("\"\"");
                        position++;
                    }
                    else
                    {
                        Text(str, position, nextPosition - position);
                        Text("\"\"");
                        position = nextPosition + 1;
                    }
                }
            }

            Text('"');
        }
    }
}