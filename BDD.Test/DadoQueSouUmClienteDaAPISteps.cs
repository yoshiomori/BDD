using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using TechTalk.SpecFlow;

namespace BDD.Test
{
    class JsonValue
    {
        protected dynamic value;
        public virtual dynamic Value
        {
            get => value;
            set
            {
                switch (value.Type)
                {
                    case JTokenType.Array:
                        this.value = value.ToObject<IList<JToken>>();
                        break;
                    case null:
                        this.value = value.ToObject<IDictionary<string, JToken>>();
                        break;
                    case JTokenType.Integer:
                        this.value = value.ToObject<int>();
                        break;
                    case JTokenType.Float:
                        this.value = value.ToObject<float>();
                        break;
                    case JTokenType.String:
                        this.value = value.ToObject<string>();
                        break;
                    case JTokenType.Date:
                        this.value = value.ToObject<DateTime>();
                        break;
                    case JTokenType.Null:
                        this.value = null;
                        break;
                    default:
                        throw new NotImplementedException("Value: " + value);
                }
            }
        }

        public JsonValue(string json)
        {
            Value = JToken.Parse(json);
        }

        public JsonValue() { }

        public virtual JsonValue this[string key]
        {
            get
            {
                return new JsonValue() { Value = Value[key] };
            }
        }

        public virtual JsonValue this[int index]
        {
            get
            {
                return new JsonValue { Value = Value[index] };
            }
        }

        public void AssertIsValid(string expectedJson)
        {
            var typeOfDictionary = typeof(Dictionary<string, JToken>);
            var typeOfList = typeof(List<JToken>);
            var stackExpected = new Stack<JsonValue>();
            var stackResponse = new Stack<JsonValue>();
            var jsonNotIsTheSame = "O json não é o esperado.";
            stackExpected.Push(new JsonValue(expectedJson));
            stackResponse.Push(this);
            while (stackExpected.Count > 0)
            {
                JsonValue expected = stackExpected.Pop();
                JsonValue response = stackResponse.Pop();
                if (!(expected.Value is null) && expected.Value.GetType() == typeOfDictionary)
                {
                    Assert.IsInstanceOfType(response.Value, typeOfDictionary, jsonNotIsTheSame);
                    foreach (var key in expected.Value.Keys)
                    {
                        Assert.IsTrue(response.Value.ContainsKey(key), jsonNotIsTheSame);
                        stackExpected.Push(expected[key]);
                        stackResponse.Push(response[key]);
                    }
                }
                else if (!(expected.Value is null) && expected.Value.GetType() == typeof(List<JToken>))
                {
                    Assert.IsInstanceOfType(response.Value, typeOfList, jsonNotIsTheSame);
                    for (int i = 0; i < expected.Value.Count; i++)
                    {
                        JsonValue valueExpected = expected[i];
                        JsonValue valueResponse = response[i];
                        stackExpected.Push(valueExpected);
                        stackResponse.Push(valueResponse);
                    }
                }
                else
                {
                    if (expected.Value is null)
                    {
                        Assert.IsNull(response.Value);
                    }
                    else if (response.Value is null)
                    {
                        Assert.IsNotNull(response.Value, "Actual: null and Expected: " + expected.Value.GetType().ToString());
                    }
                    else
                    {
                        Assert.IsInstanceOfType(
                            response.Value,
                            GetJsonToken(expected.Value),
                            expected.Value.ToString() + " " + response.Value.ToString()
                        );
                    }
                }
            }
        }

        private Type GetJsonToken(string jsonToken)
        {
            Type @typeof = null;
            switch (jsonToken)
            {
                case "String":
                    @typeof = typeof(string);
                    break;
                case "Integer":
                    @typeof = typeof(int);
                    break;
                case "Boolean":
                    @typeof = typeof(bool);
                    break;
                case "Float":
                    @typeof = typeof(float);
                    break;
                case "Date":
                    @typeof = typeof(DateTime);
                    break;
                case "DateTime":
                    @typeof = typeof(DateTime);
                    break;
                default:
                    Assert.Fail("JsonToken does not exist: " + jsonToken);
                    break;
            }
            return @typeof;
        }
    }

    [Binding]
    public class DadoQueSouUmClienteDaAPISteps
    {
        private const string RespostaInvalida = "O tipo da resposta não é válido";
        private string token;

        [Given(@"que a url do endpoint é '(.*)'")]
        public void DadoQueAUrlDoEndpointE(string url)
        {
            ScenarioContext.Current["Endpoint"] = url;
        }
        
        [Given(@"o metodo é '(.*)'")]
        public void DadoOMetodoE(string p0)
        {
            var metodo = Method.POST;

            switch (p0.ToUpper())
            {
                case "GET":
                    metodo = Method.GET;
                    break;
                case "POST":
                    metodo = Method.POST;
                    break;
                case "PUT":
                    metodo = Method.PUT;
                    break;
                case "DELETE":
                    metodo = Method.DELETE;
                    break;
                case "HEAD":
                    metodo = Method.HEAD;
                    break;
                case "OPTIONS":
                    metodo = Method.OPTIONS;
                    break;
                case "PATCH":
                    metodo = Method.PATCH;
                    break;
                case "MERGE":
                    metodo = Method.MERGE;
                    break;
                case "COPY":
                    metodo = Method.COPY;
                    break;
                default:
                    Assert.Fail("Método HTTP não esperado");
                    break;
            }

            ScenarioContext.Current["HttpMethod"] = metodo;
        }
        
        [When(@"chamar o serviço")]
        public void QuandoChamarOServico()
        {
            var endpoint = (string)ScenarioContext.Current["Endpoint"];

            ExecutarRequest(endpoint);
        }
        
        [Then(@"statuscode da resposta deverá ser '(.*)'")]
        public void EntaoStatuscodeDaRespostaDeveraSer(string p0)
        {
            var response = (IRestResponse)ScenarioContext.Current["Response"];

            string erroMessage;

            switch (response.StatusCode)
            {

                case System.Net.HttpStatusCode.InternalServerError:
                case System.Net.HttpStatusCode.NotFound:
                    erroMessage = string.Format("ResponseURI {0}", response.ResponseUri);
                    break;
                case System.Net.HttpStatusCode.Forbidden:
                    var auth = response.Request.Parameters.Where(x => x.Name == "Authorization").FirstOrDefault();
                    erroMessage = string.Format("Authorization: {0}", auth != null ? auth.Value : "none");
                    break;
                default:
                    erroMessage = response.Content;
                    break;
            }

            Assert.AreEqual(p0, response.StatusCode.ToString(), erroMessage);
        }

        [Then(@"a resposta deve ser (.+)")]
        public void EntaoARespostaDeveSer(string expectedJson)
        {
            var jsonResponse = new JsonValue(((IRestResponse)ScenarioContext.Current["Response"]).Content);
            jsonResponse.AssertIsValid(expectedJson);
        }

        #region private

        private void ExecutarRequest(string endpoint)
        {
            var request = new RestRequest();

            request.Method = (Method)ScenarioContext.Current["HttpMethod"];
            request.Parameters.Clear();

            if (request.Method == Method.POST || request.Method == Method.PUT)
            {
                var json = (string)ScenarioContext.Current["Data"];

                if (!string.IsNullOrWhiteSpace(json))
                {
                    request.AddParameter("application/json", json, ParameterType.RequestBody);
                }
            }

            var restClient = new RestClient(endpoint);
            var response = restClient.Execute(request);

            if (response.ErrorException != null)
            {
                const string message = "Error retrieving response.  Check inner details for more info.";
                throw new ApplicationException(message, response.ErrorException);
            }

            ScenarioContext.Current["Response"] = response;
        }

        #endregion
    }
}
