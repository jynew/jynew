using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;

namespace UniRx
{
    public static class WebRequestExtensions
    {
        static IObservable<TResult> AbortableDeferredAsyncRequest<TResult>(Func<AsyncCallback, object, IAsyncResult> begin, Func<IAsyncResult, TResult> end, WebRequest request)
        {
            var result = Observable.Create<TResult>(observer =>
            {
                var isCompleted = -1;
                var subscription = Observable.FromAsyncPattern<TResult>(begin,
                    ar =>
                    {
                        try
                        {
                            Interlocked.Increment(ref isCompleted);
                            return end(ar);
                        }
                        catch (WebException ex)
                        {
                            if (ex.Status == WebExceptionStatus.RequestCanceled) return default(TResult);
                            throw;
                        }
                    })()
                    .Subscribe(observer);
                return Disposable.Create(() =>
                {
                    if (Interlocked.Increment(ref isCompleted) == 0)
                    {
                        subscription.Dispose();
                        request.Abort();
                    }
                });
            });

            return result;
        }

        public static IObservable<WebResponse> GetResponseAsObservable(this WebRequest request)
        {
            return AbortableDeferredAsyncRequest<WebResponse>(request.BeginGetResponse, request.EndGetResponse, request);
        }

        public static IObservable<HttpWebResponse> GetResponseAsObservable(this HttpWebRequest request)
        {
            return AbortableDeferredAsyncRequest<HttpWebResponse>(request.BeginGetResponse, ar => (HttpWebResponse)request.EndGetResponse(ar), request);
        }

        public static IObservable<Stream> GetRequestStreamAsObservable(this WebRequest request)
        {
            return AbortableDeferredAsyncRequest<Stream>(request.BeginGetRequestStream, request.EndGetRequestStream, request);
        }
    }
}