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
                using DeeDee.Models;
                namespace {ns}DeeDee.Generated.Models"
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

        private static void ExtensionIRequest
        (
            ref StringBuilder sourceBuilder,
            List<(string RequestClassName, bool IsAsync)> irequests
        )
        {
            foreach (var (requestClassName, isAsync) in irequests)
            {
                if (isAsync)
                {
                    sourceBuilder.AppendFormat
                    (@"
                        public Task SendAsync({0} request, CancellationToken token = default);", requestClassName
                    ).AppendLine();
                }
                else
                {
                    sourceBuilder.AppendFormat
                    (@"
                        public void Send({0} request);", requestClassName
                    ).AppendLine();
                }

            }

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
                    sourceBuilder.AppendFormat
                    (@"
                        public Task SendAsync({0} request, CancellationToken token = default);", requestClassName
                    ).AppendLine();
                }
                else
                {
                    sourceBuilder.AppendFormat
                    (@"
                        public void Send({0} request);", requestClassName
                    ).AppendLine();
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
                    sourceBuilder.AppendFormat
                    (@"
                        public Task<{0}> SendAsync
                        (
                            {1} request,
                            CancellationToken token = default
                        );", responseClassName, requestClassName
                    ).AppendLine();
                }
                else
                {
                    sourceBuilder.AppendFormat
                    (@"
                        public {0} Send
                        (
                            {1} request
                        );", responseClassName, requestClassName
                    ).AppendLine();
                }
            }

        }
    }


}
