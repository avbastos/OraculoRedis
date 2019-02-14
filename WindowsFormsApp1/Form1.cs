using Microsoft.Azure.CognitiveServices.Search.WebSearch;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        ConnectionMultiplexer redis = null;
        IDatabase database = null;

        public Form1()
        {
            InitializeComponent();
            Connect();
        }

        public void Connect()
        {

            redis = ConnectionMultiplexer.Connect("40.122.106.36");
            database = redis.GetDatabase();
            var sub = redis.GetSubscriber();
            sub.Subscribe(new RedisChannel("perguntas", RedisChannel.PatternMode.Auto), (ch, msg) =>
            {
                Pesquisa(msg);
            });
        }

        public void Pesquisa(string question)
        {
            //string question = "Cotação Dollar";
            //string question = "Capital da Bahia";
            //string question = "2+2";
            var client = new WebSearchClient(new ApiKeyServiceClientCredentials("cdab4adbb42a487a9e3423a4ec716739"));
            string answer = BingSearch.WebResults(client, question);

            database.HashSet(question.Substring(0,4), "SHAZAN", answer);
        }
    }
}
