using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Net;
using System.Web;
using DevDefined.OAuth.Consumer;
using DevDefined.OAuth.Framework;
using Newtonsoft.Json;
using StorEdge.Api.Responses;

namespace StorEdge.Api {
    public class BaseReader {
        const int API_VERSION = 1;

        private String key;
        private String secret;

        protected StorEdge.Api.Types.Meta last_meta = null;
        protected int _page = 0;
        protected int _per_page = 0;
        protected Uri _uri;
        protected Type _response_type;

        public String endpoint;
        public NameValueCollection parameters = HttpUtility.ParseQueryString(String.Empty);

        public BaseReader(String key, String secret) {
            this.key = key;
            this.secret = secret;

                endpoint = StorEdge.Api.Properties.Settings.Default.endpoint;
        }

        public BaseReader page(int page) {
            _page = page;
            last_meta = null;
            return this;
        }

        public BaseReader per_page(int per_page) {
            _per_page = per_page;
            last_meta = null;
            return this;
        }

        public Int32 next_page() {
            if (last_meta != null && last_meta.pagination != null) {
                return last_meta.pagination.next_page;
            }
            else {
                return _page == 0 ? 1 : _page;
            }
        }

        public Boolean has_next_page() {
            return next_page() > 0;
        }

        public BaseReader parameter(String key, String value) {
            parameters[key] = value;
            return this;
        }

        public BaseReader parameter(String key, int value) {
            parameters[key] = value.ToString();
            return this;
        }

        public BaseReader define_request(String path, Type response_type) {
            Uri uri = new Uri(String.Format("{0}/v{1}/{2}.json", endpoint, API_VERSION, path));
            if (_uri != uri) {
                _response_type = response_type;
                last_meta = null;
            }
            _uri = uri;
            return this;
        }

        public void reset() {
            last_meta = null;
        }

        public BaseResponse read_next_page() {
            if (!has_next_page()) {
                return null;
            }

            parameter("page", next_page());
            if (_per_page != 0) {
                parameter("per_page", _per_page);
            }

            var session = new OAuthSession(oath_context(), _uri, _uri, _uri);
            var request = session.Request().Get().ForUri(parameterize_uri());
            BaseResponse response = (BaseResponse)JsonConvert.DeserializeObject(request.ToString(), _response_type, json_settings());
            if (response.meta.status_code != 200) {
                throw new Exception("Error talking to StorEdge: " + response.meta.status_message);
            }
            last_meta = response.meta;
            return response;
        }

        private JsonSerializerSettings json_settings() {
            return new JsonSerializerSettings {
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Ignore,
                DateFormatHandling = DateFormatHandling.IsoDateFormat
            };
        }

        private Uri parameterize_uri() {
            return new Uri(String.Format("{0}?{1}", _uri.ToString(), parameters.ToString()));
        }

        private OAuthConsumerContext oath_context() {
            return new OAuthConsumerContext {
                ConsumerKey = key,
                ConsumerSecret = secret,
                SignatureMethod = SignatureMethod.HmacSha1,
                UseHeaderForOAuthParameters = true
            };
        }
    }
}
