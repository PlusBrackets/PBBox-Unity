/*--------------------------------------------------------
 *Copyright (c) 2022 PlusBrackets
 *@update: 2022.04.13
 *@author: PlusBrackets
 --------------------------------------------------------*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Networking;

namespace PBBox.Network
{

    public class PBNetwork : SingleBehaviour<PBNetwork>
    {


        #region static property
        /// <summary>
        /// 如果有请求出现错误时调用
        /// </summary>
        public static Action<long, string> onRequestError;
        /// <summary>
        /// 当有请求开始访问时调用
        /// </summary>
        public static Action onRequestsStart;
        /// <summary>
        /// 当前所有请求都访问完成时调用
        /// </summary>
        public static Action onRequestsFinish;
        /// <summary>
        /// 默认的timeout值
        /// </summary>
        public static int DefaultTimeout = 50;
        #endregion

        #region static func
        /// <summary>
        /// Post数据
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postData"></param>
        /// <param name="onResult">成功时调用，返回字符串数据</param>
        /// <param name="onError">发生错误时调用，返回error的信息</param>
        /// <param name="timeoutRetry">超时重试的次数</param>
        /// <param name="modifyWebRequest">自定义请求的设置</param>
        /// <param name="inQueue">是否添加到请求的队列，请求队列将会以FIFO的顺序依次请求网络,默认为否</param>
        public static long Post(string url, string postData, Action<string> onResult, Action<string> onError = null, int timeoutRetry = 1, Action<UnityWebRequest> modifyWebRequest = null, bool inQueue = false)
        {
            RequestInfo _requestInfo = new RequestInfo(url, postData, onResult, onError, timeoutRetry, modifyWebRequest);
            SingleBehaviour<PBNetwork>.Instance.DoRequest(_requestInfo, inQueue);
            return _requestInfo.ID;
        }

        /// <summary>
        /// Post数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="postData"></param>
        /// <param name="onResult">成功时调用，返回泛型数据</param>
        /// <param name="onError">发生错误时调用，返回error的信息</param>
        /// <param name="timeoutRetry">超时重试的次数</param>
        /// <param name="modifyWebRequest">自定义请求的设置</param>
        /// <param name="inQueue">是否添加到请求的队列，请求队列将会以FIFO的顺序依次请求网络,默认为否</param>
        public static long Post<T>(string url, string postData, Action<T> onResult, Action<string> onError = null, int timeoutRetry = 1, Action<UnityWebRequest> modifyWebRequest = null, bool inQueue = false)
        {
            RequestInfo _requestInfo = new RequestInfo(url, postData,
                (string reuslt) =>
                {
                    PraseRequestAndInvoke(reuslt, onResult);
                },
                onError, timeoutRetry, modifyWebRequest);
            Instance.DoRequest(_requestInfo, inQueue);
            return _requestInfo.ID;
        }

        /// <summary>
        /// Post数据
        /// </summary>
        /// <param name="url"></param>
        /// <param name="postForm"></param>
        /// <param name="onResult">成功时调用，返回字符串数据</param>
        /// <param name="onError">发生错误时调用，返回error的信息</param>
        /// <param name="timeoutRetry">超时重试的次数</param>
        /// <param name="modifyWebRequest">自定义请求的设置</param>
        /// <param name="inQueue">是否添加到请求的队列，请求队列将会以FIFO的顺序依次请求网络,默认为否</param>
        public static long Post(string url, WWWForm postForm, Action<string> onResult, Action<string> onError = null, int timeoutRetry = 1, Action<UnityWebRequest> modifyWebRequest = null, bool inQueue = false)
        {
            RequestInfo _requestInfo = new RequestInfo(url, postForm, onResult, onError, timeoutRetry, modifyWebRequest);
            Instance.DoRequest(_requestInfo, inQueue);
            return _requestInfo.ID;
        }

        /// <summary>
        /// Post数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="postForm"></param>
        /// <param name="onResult">成功时调用，返回泛型数据</param>
        /// <param name="onError">发生错误时调用，返回error的信息</param>
        /// <param name="timeoutRetry">超时重试的次数</param>
        /// <param name="modifyWebRequest">自定义请求的设置</param>
        /// <param name="inQueue">是否添加到请求的队列，请求队列将会以FIFO的顺序依次请求网络,默认为否</param>
        public static long Post<T>(string url, WWWForm postForm, Action<T> onResult, Action<string> onError = null, int timeoutRetry = 1, Action<UnityWebRequest> modifyWebRequest = null, bool inQueue = false)
        {
            RequestInfo _requestInfo = new RequestInfo(url, postForm,
                (string reuslt) =>
                {
                    PraseRequestAndInvoke(reuslt, onResult);
                },
                onError, timeoutRetry, modifyWebRequest);
            Instance.DoRequest(_requestInfo, inQueue);
            return _requestInfo.ID;
        }

        /// <summary>
        /// Get数据
        /// </summary>
        /// <param name="url"></param>
        /// <param name="onResult">成功时调用，返回字符串数据</param>
        /// <param name="onError">发生错误时调用，返回error的信息</param>
        /// <param name="timeoutRetry">超时重试的次数</param>
        /// <param name="modifyWebRequest">自定义请求的设置</param>
        /// <param name="inQueue">是否添加到请求的队列，请求队列将会以FIFO的顺序依次请求网络,默认为否</param>
        public static long Get(string url, Action<string> onResult, Action<string> onError = null, int timeoutRetry = 1, Action<UnityWebRequest> modifyWebRequest = null, bool inQueue = false)
        {
            RequestInfo _requestInfo = new RequestInfo(url, onResult, onError, timeoutRetry, modifyWebRequest);
            Instance.DoRequest(_requestInfo, inQueue);
            return _requestInfo.ID;
        }

        /// <summary>
        /// Get数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="onResult">成功时调用，返回泛型数据</param>
        /// <param name="onError">发生错误时调用，返回error的信息</param>
        /// <param name="timeoutRetry">超时重试的次数</param>
        /// <param name="modifyWebRequest">自定义请求的设置</param>
        /// <param name="inQueue">是否添加到请求的队列，请求队列将会以FIFO的顺序依次请求网络,默认为否</param>
        public static long Get<T>(string url, Action<T> onResult, Action<string> onError = null, int timeoutRetry = 1, Action<UnityWebRequest> modifyWebRequest = null, bool inQueue = false)
        {
            RequestInfo _requestInfo = new RequestInfo(url,
                 (string reuslt) =>
                 {
                     PraseRequestAndInvoke(reuslt, onResult);
                 },
                 onError, timeoutRetry, modifyWebRequest);
            Instance.DoRequest(_requestInfo, inQueue);
            return _requestInfo.ID;
        }

        private static void PraseRequestAndInvoke<T>(string json, Action<T> callback)
        {
            if (callback != null)
            {
                //DebugUtils.Internal.Log(json);
                T _data = JsonUtility.FromJson<T>(json);//LitJson.JsonMapper.ToObject<T>(json);
                                                        // T _data = JsonUtility.FromJson<T>(json);
                callback.Invoke(_data);
            }
        }
        #endregion

        #region private class
        private enum RequestType
        {
            /// <summary>
            /// Post string类型的数据
            /// </summary>
            Post_String,
            /// <summary>
            /// Post WWWForm类型的数据
            /// </summary>
            Post_Form,
            /// <summary>
            /// 使用Get请求数据
            /// </summary>
            Get,
        }

        private class RequestInfo
        {
            private static long ID_TEMPLET = 0;

            public long ID
            {
                get;
                private set;
            }
            private RequestType requestType;
            private string url;
            private string postData;
            private WWWForm postForm;
            public Action<string> onResult;
            public Action<string> onError;
            private Action<UnityWebRequest> modifyRequest;
            private UnityWebRequest request = null;
            public bool isReqeustExisted
            {
                get
                {
                    return request != null;
                }
            }

            public int timeoutRetry;

            public RequestInfo(string url, Action<string> onRequest, Action<string> onError, int timeoutRetry, Action<UnityWebRequest> modifyRequest)
            {
                requestType = RequestType.Get;
                InitProperty(url, onRequest, onError, timeoutRetry, modifyRequest);
            }

            public RequestInfo(string url, string postData, Action<string> onRequest, Action<string> onError, int timeoutRetry, Action<UnityWebRequest> modifyRequest)
            {
                requestType = RequestType.Post_String;
                this.postData = postData;
                InitProperty(url, onRequest, onError, timeoutRetry, modifyRequest);
            }

            public RequestInfo(string url, WWWForm postForm, Action<string> onRequest, Action<string> onError, int timeoutRetry, Action<UnityWebRequest> modifyRequest)
            {
                requestType = RequestType.Post_Form;
                this.postForm = postForm;
                InitProperty(url, onRequest, onError, timeoutRetry, modifyRequest);
            }

            private void InitProperty(string url, Action<string> onRequest, Action<string> onError, int timeoutRetry, Action<UnityWebRequest> modifyRequest)
            {
                ID = ID_TEMPLET++;
                this.url = url;
                this.onResult = onRequest;
                this.onError = onError;
                this.modifyRequest = modifyRequest;
                this.timeoutRetry = timeoutRetry;
            }

            public UnityWebRequest GetRequest()
            {
                if (request == null)
                {
                    switch (requestType)
                    {
                        case RequestType.Get:
                            request = UnityWebRequest.Get(url);
                            break;
                        case RequestType.Post_String:
                            request = UnityWebRequest.Post(url, postData);
                            break;
                        case RequestType.Post_Form:
                            request = UnityWebRequest.Post(url, postForm);
                            break;
                    }

                    //共用的request自定义设置
                    request.timeout = DefaultTimeout;

                    //单个调用的更改reqeust的设置
                    if (modifyRequest != null)
                    {
                        modifyRequest.Invoke(request);
                    }
                }
                return request;
            }

            public void DisposeRequest()
            {
                request.Dispose();
                request = null;
            }
        }
        #endregion

        /// <summary>
        /// 储存请求信息的字典
        /// </summary>
        private Dictionary<long, RequestInfo> m_DictRequestInfo;
        /// <summary>
        /// 请求信息的队列，请求队列将会以FIFO的顺序依次请求网络
        /// </summary>
        private List<long> m_ListRequestWaiting;
        /// <summary>
        /// 正在访问网络的请求的队列
        /// </summary>
        private List<long> m_ListRequestSending;
        /// <summary>
        /// 队列是否正在请求中
        /// </summary>
        private bool m_IsQueueRequesting = false;
        
        protected override void InitAsInstance()
        {
            base.InitAsInstance();
            m_DictRequestInfo = new Dictionary<long, RequestInfo>();
            m_ListRequestWaiting = new List<long>();
            m_ListRequestSending = new List<long>();
        }

        /// <summary>
        /// 根据请求信息做请求操作
        /// </summary>
        /// <param name="requestInfo"></param>
        /// <param name="inQueue">是否添加进请求队列</param>
        private void DoRequest(RequestInfo requestInfo, bool inQueue)
        {
            m_DictRequestInfo.Add(requestInfo.ID, requestInfo);
            if (inQueue)
            {
                //将请求信息添加至队列中，如果未开启队列的网络访问的协程则开启
                m_ListRequestWaiting.Add(requestInfo.ID);
                if (!m_IsQueueRequesting)
                    StartCoroutine(WaitForRequestQueue());
            }
            else
            {
                //直接请求网络并返回数据
                StartCoroutine(WaitForSendRequestAndCallBack(requestInfo.ID));
            }
        }

        /// <summary>
        /// 等待请求队列完成
        /// </summary>
        /// <returns></returns>
        private IEnumerator WaitForRequestQueue()
        {
            m_IsQueueRequesting = true;
            while (m_ListRequestWaiting.Count > 0)
            {
                //FIFO
                long _requestInfoID = m_ListRequestWaiting[0];
                m_ListRequestWaiting.RemoveAt(0);
                yield return WaitForSendRequestAndCallBack(_requestInfoID);
            }
            m_IsQueueRequesting = false;
        }

        /// <summary>
        /// 等待发送请求并callback结果
        /// </summary>
        /// <param name="requestInfo"></param>
        /// <returns></returns>
        private IEnumerator WaitForSendRequestAndCallBack(long requestInfoID)
        {
            int timeoutCount = 0;
            //如果正在发送的请求数量为0，则调用onRequestsStart
            if (m_ListRequestSending.Count == 0)
            {
                if (onRequestsStart != null)
                    onRequestsStart.Invoke();
            }
            m_ListRequestSending.Add(requestInfoID);

            RequestInfo _requestInfo = m_DictRequestInfo[requestInfoID];
            while (true)
            {

                UnityWebRequest _request = _requestInfo.GetRequest();
                //Debug.Log("Wait Reuqest " + requestInfoID + ", URL:" + _request.url);
                yield return _request.SendWebRequest();
                // 处理结果
                if (_request.result != UnityWebRequest.Result.Success)
                {
                    //Debug.Log(_request.error);
                    if (_request.error.Equals("Request timeout") && timeoutCount < _requestInfo.timeoutRetry)
                    {
                        //Debug.Log("retry "+(timeoutCount+1));
                        timeoutCount++;
                        _requestInfo.DisposeRequest();
                        continue;
                    }
                    if (_requestInfo.onError != null)
                    {
                        _requestInfo.onError.Invoke(_request.error);
                    }
                    if (onRequestError != null)
                    {
                        onRequestError.Invoke(_requestInfo.ID, _request.error);
                    }
                    break;
                }
                else
                {
                    if (_requestInfo.onResult != null)
                    {
                        _requestInfo.onResult.Invoke(_request.downloadHandler.text);
                    }
                    break;
                }
            }
            //如果正在发送的请求数量为0，则调用onRequestsFinish
            m_ListRequestSending.Remove(requestInfoID);
            //Debug.Log("request end " + requestInfoID);
            if (m_ListRequestSending.Count == 0)
            {
                //Debug.Log("Count Zero");
                if (onRequestsFinish != null)
                    onRequestsFinish.Invoke();
            }
            //完成请求后移除Request 
            m_DictRequestInfo.Remove(requestInfoID);
        }
    }
}