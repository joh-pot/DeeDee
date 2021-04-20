using System.Collections.Generic;
using System.Text;

namespace DeeDee.Builders
{
    internal static class DispatcherInterfaceBuilder
    {
        public static string Build
        (
            string ns,
            List<(string RequestClassName, bool IsAsync)> irequests,
            List<(string RequestClassName, string ResponseClassName, bool IsAsync)> irequestsOfT
        )
        {
            var sourceBuilder = new StringBuilder
            ($@"
                using System;
                using System.Threading;
                using System.Threading.Tasks;
                namespace {ns}DeeDee"
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
            foreach (var (requestClassName, isAsync) in irequests)
            {
                if (isAsync)
                {
                    sourceBuilder.AppendLine
                    ($@"
                        public Task SendAsync({requestClassName} request, CancellationToken token = default);"
                    );
                }
                else
                {
                    sourceBuilder.AppendLine
                    ($@"
                        public void Send({requestClassName} request);"
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
            foreach (var (requestClassName, responseClassName, isAsync) in irequestsOfT)
            {
                if (isAsync)
                {
                    sourceBuilder.AppendLine
                    ($@"
                        public Task<{responseClassName}> SendAsync
                        (
                            {requestClassName} request,
                            CancellationToken token = default
                        );"
                    );
                }
                else
                {
                    sourceBuilder.AppendLine
                    ($@"
                        public {responseClassName} Send
                        (
                            {requestClassName} request
                        );"
                    );
                }
            }

        }
    }


}
