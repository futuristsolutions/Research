using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using Oracle.DataAccess.Client;

namespace OracleHarness
{
    class Program
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Program));
        private static readonly string ConnectionString = string.Format(
"Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST={0})(PORT=1521))(CONNECT_DATA=(SERVICE_NAME={1})));User Id={2};Password={3};Pooling=true",
"localhost", "xe", "cs_scheduler", "tesco");
        private const int NoMessagesErrorCode = 25228;
        static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();

            if (args.Any())
            {
                var parts = args.First().Split(new[] {'='});
                if (parts.First().Contains("dq"))
                {
                    int algo = Convert.ToInt32(parts.Last());
                    DequeueTest(algo);
                }
                else if (parts.First().Contains("eq"))
                {
                    int delay = 0;
                    int messageCount = 5;
                    bool loop = false;
                    if (parts.Count() == 2)
                    {
                        messageCount = Convert.ToInt32(parts.Last());
                    }
                    if (args.Count() > 2 && args.Skip(1).Take(1).FirstOrDefault().Contains("delay")) 
                    {
                        parts = args.Skip(1).Take(1).FirstOrDefault().Split(new[] { '=' });
                        if (parts.Count() == 2)
                        {
                            delay = Convert.ToInt32(parts.Last());
                        }
                        loop = args.Last().Contains("loop");
                    }

                    do
                    {
                        EnqueueData(messageCount, delay);
                        var waitTime = delay/2;
                        Log.InfoFormat("Waiting for {0} seconds", waitTime);
                        Thread.Sleep(TimeSpan.FromSeconds(waitTime));
                    } while (loop);

                }
            }
            else
            {
                Console.WriteLine("Usage: -dq=algo(1|2){1=Listen,2=NoListen} | -eq=n{number of message} [-delay=n{number of second of message delay}]");
            }


        }
        
        private static void DequeueTest(int algo)
        {
            var action = (algo == 1)
                ? (Action<int>) DequeueTest_UsingListen
                : DequeueTest_NoListen;
            var tasks = new List<Task>();
            for (int index = 1; index <= 32; index++)
            {
                var identifier = index;
                var task = Task.Factory.StartNew(() => action(identifier), TaskCreationOptions.LongRunning);
                tasks.Add(task);
            }

            Log.InfoFormat("{0} - {1}", action.Method.Name, tasks.Count);
            Task.WaitAll(tasks.ToArray());
        }

        private static void EnqueueData(int messageCount, int delay)
        {
            Log.InfoFormat("Enqueue {0} message(s) with delay seconds {1}", messageCount, delay);
            var connection = new OracleConnection(ConnectionString);
            connection.Open();
            var group = Guid.NewGuid().ToString();
            for (int index = 1; index <= messageCount; index++)
            {
                var data = string.Format("Test message : {0} - {1}", group, index);
                var message = new OracleAQMessage
                {

                    Payload = Encoding.UTF8.GetBytes(data),
                    Correlation = index.ToString(),
                    Delay = delay
                };

                var queue = new OracleAQQueue("SCHEDULE_QUEUE", connection)
                {
                    MessageType = OracleAQMessageType.Raw,
                    EnqueueOptions =
                    {
                        Visibility = OracleAQVisibilityMode.OnCommit,
                        DeliveryMode = OracleAQMessageDeliveryMode.Persistent
                    }
                };
                Log.InfoFormat("Enqueueing {0}", data);
                var transaction = connection.BeginTransaction();
                queue.Enqueue(message);
                transaction.Commit();
            }
        }

        private static void DequeueTest_UsingListen(int identifier)
        {
            try
            {
                var aqueueAgent = new [] {new OracleAQAgent(null,"CS_SCHEDULER.SCHEDULE_QUEUE")};
                var connection = new OracleConnection(ConnectionString);
                connection.Open();
                while (true)
                {
                    var agent = OracleAQQueue.Listen(connection, aqueueAgent, 1);
                    if (agent != null)
                    {
                        Dequeue(connection, identifier);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message, e);
                Log.ErrorFormat("Identifier : {0}", identifier);
                Environment.Exit(0);
            }
           
        }

        private static void DequeueTest_NoListen(int identifier)
        {
            try
            {
                var connection = new OracleConnection(ConnectionString);
                connection.Open();
                while (true)
                {
                    Dequeue(connection, identifier);
                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message,e);
                throw;
            }

        }


        private static void Dequeue(OracleConnection connection, int identifier)
        {
            var queue = new OracleAQQueue("SCHEDULE_QUEUE", connection)
            {
                MessageType = OracleAQMessageType.Raw,
                EnqueueOptions =
                {
                    Visibility = OracleAQVisibilityMode.OnCommit,
                    DeliveryMode = OracleAQMessageDeliveryMode.Persistent
                }
            };
            var transaction = connection.BeginTransaction();
            var dequeueOptions = new OracleAQDequeueOptions
            {
                Visibility = OracleAQVisibilityMode.OnCommit,
                NavigationMode = OracleAQNavigationMode.NextMessage,
                DequeueMode = OracleAQDequeueMode.Remove,
                DeliveryMode = OracleAQMessageDeliveryMode.Persistent,
                Wait = 1
            };
            try
            {
                var message = queue.Dequeue(dequeueOptions);
                if (message != null)
                {
                    var data = Encoding.UTF8.GetString((byte[])message.Payload);
                    Log.InfoFormat("[{0:D2}] : {1} ",identifier, data);
                }
            }
            catch (OracleException ex)
            {
                if (ex.Number != NoMessagesErrorCode)
                    throw;
            }
            transaction.Commit();
            transaction.Dispose();
            queue.Dispose();
        }
    }
}
