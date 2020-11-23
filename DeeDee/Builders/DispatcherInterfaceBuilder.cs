using System.Collections.Generic;
using System.Text;

namespace DeeDee.Builders
{
    internal static class DispatcherInterfaceBuilder
    {
        public static string Build
        (
            List<(string RequestClassName, bool IsAsync)> irequests,
            List<(string RequestClassName, string ResponseClassName, bool IsAsync)> irequestsOfT
        )
        {
            var sourceBuilder = new StringBuilder
            (@"
                using System;
                using System.Threading;
                using System.Threading.Tasks;
                namespace DeeDee"
            );
            sourceBuilder.AppendLine("{");
            sourceBuilder.AppendLine("public interface IDispatcher");
            sourceBuilder.AppendLine("{");
            SignaturesIRequest(ref sourceBuilder, irequests);
            SignaturesIRequestT(ref sourceBuilder, irequestsOfT);
            sourceBuilder.AppendLine("}");

            sourceBuilder.AppendLine("}");
            return sourceBuilder.ToString();
        }

        private static void SignaturesIRequest
        (
            ref StringBuilder sourceBuilder,
            List<(string RequestClassName, bool IsAsync)> irequests
        )
        {
            foreach (var (RequestClassName, IsAsync) in irequests)
            {
                if (IsAsync)
                {
                    sourceBuilder.AppendLine
                    ($@"
                        public Task SendAsync({RequestClassName} request, CancellationToken token = default);"
                    );
                }
                else
                {
                    sourceBuilder.AppendLine
                    ($@"
                        public void Send({RequestClassName} request);"
                    );
                }

            }

        }

        private static void SignaturesIRequestT
        (
            ref StringBuilder sourceBuilder,
            List<(string RequestClassName, string ResponseClassName, bool IsAsync)> irequestsOfT
        )
        {
            foreach (var (RequestClassName, ResponseClassName, IsAsync) in irequestsOfT)
            {
                if (IsAsync)
                {
                    sourceBuilder.AppendLine
                    ($@"
                        public Task<{ResponseClassName}> SendAsync
                        (
                            {RequestClassName} request,
                            CancellationToken token = default
                        );"
                    );
                }
                else
                {
                    sourceBuilder.AppendLine
                    ($@"
                        public {ResponseClassName} Send
                        (
                            {RequestClassName} request
                        );"
                    );
                }
            }

        }
    }


}
