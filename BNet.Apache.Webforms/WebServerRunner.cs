using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Hosting;
using System.Collections.Specialized;
using System.Reflection;

namespace BNet.Apache.Webforms
{
    public class WebServerRunner
    {
        private static HttpListener _listener;
        private static CancellationTokenSource _cts;

        public static void StartServer(int port = 8180, string virtualDir = "/")
        {
            _cts = new CancellationTokenSource();

            Task.Run(() =>
            {
                string prefix = $"http://+:{port}/";

                try
                {
                    PortManager.AddUrlAcl(prefix);
                    PortManager.OpenFirewallPort(port, "ASPHosting_Port_" + port);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Port config error: " + ex.Message);
                }

                // Determine physical directory where your web forms files are located
                string exeDir = AppDomain.CurrentDomain.BaseDirectory;
                string physicalDir;
                if (Path.GetFileName(exeDir).Equals("bin", StringComparison.OrdinalIgnoreCase))
                    physicalDir = Directory.GetParent(exeDir).FullName + Path.DirectorySeparatorChar;
                else
                    physicalDir = exeDir + Path.DirectorySeparatorChar;

                WebHost host;
                try
                {
                    host = (WebHost)ApplicationHost.CreateApplicationHost(
                        typeof(WebHost), virtualDir, physicalDir);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("AppHost init failed: " + ex.Message);
                    return;
                }

                _listener = new HttpListener();
                _listener.Prefixes.Add(prefix);

                try
                {
                    _listener.Start();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Listener start error: " + ex.Message);
                    return;
                }

                while (!_cts.Token.IsCancellationRequested)
                {
                    try
                    {
                        var contextTask = _listener.GetContextAsync();
                        contextTask.Wait(_cts.Token);
                        var ctx = contextTask.Result;

                        ThreadPool.QueueUserWorkItem(_ =>
                        {
                            try
                            {
                                // Call your existing Handle method logic here
                                // (Pass along ctx, host, physicalDir, virtualDir, port)
                            }
                            catch { }
                            finally
                            {
                                try { ctx.Response.Close(); } catch { }
                            }
                        });
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
                    catch { }
                }
            }, _cts.Token);
        }

        public static void StopServer()
        {
            _cts?.Cancel();
            _listener?.Stop();
        }
    }
}