using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace KPUScrapper.Helper
{
    class Downloader
    {
        private const string DIR = @"KPUScrapper\Result\";

        public void Download(string dir)
        {
            try
            {
                foreach (var prov in DownloadProvinsiList())
                {
                    // create prov dir
                    string provdir = dir + @"\" + DIR + prov.Key;
                    if (!Directory.Exists(provdir))
                    {
                        Directory.CreateDirectory(provdir);
                    }
                    foreach (var kab in DownloadKab(prov.Value))
                    {
                        // create kab dir
                        string kabdir = provdir + @"\" + kab.Key;
                        if (!Directory.Exists(kabdir))
                        {
                            Directory.CreateDirectory(kabdir);
                        }
                        foreach (var kec in DownloadKec(prov.Value, kab.Value))
                        {
                            // create kec dir
                            string kecdir = kabdir + @"\" + kec.Key;
                            if (!Directory.Exists(kecdir))
                            {
                                Directory.CreateDirectory(kecdir);
                            }
                            foreach (var kel in DownloadDesa(kab.Value, kec.Value))
                            {
                                // create kel dir
                                string desdir = kecdir + @"\" + kel.Key;
                                if (!Directory.Exists(desdir))
                                {
                                    Directory.CreateDirectory(desdir);
                                }
                                foreach (var TPS in DownloadTPS(kec.Value, kel.Value))
                                {
                                    if (!String.IsNullOrEmpty(TPS.Value) || !String.IsNullOrWhiteSpace(TPS.Value))
                                    {
                                        // create tps dir
                                        String tpsdir = desdir + @"\TPS-" + TPS.Key;
                                        if (!Directory.Exists(tpsdir))
                                        {
                                            Directory.CreateDirectory(tpsdir);
                                        }
                                        WebClient downloadFileClient = new WebClient();

                                        bool downloaded = false;
                                        while (!downloaded)
                                        {
                                            try
                                            {
                                                downloadFileClient.DownloadFile(TPS.Value, tpsdir + @"\ScanC1-1.zip");
                                                TulisLog(dir + @"\" + DIR, prov.Key, kab.Key, kec.Key, kel.Key, TPS.Key, true);
                                                downloaded = true;
                                            }
                                            catch
                                            {
                                                downloaded = false;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        TulisLog(dir + @"\" + DIR, prov.Key, kab.Key, kec.Key, kel.Key, TPS.Key, false);
                                    }
                                }
                            }
                        }
                    }
                }

                //DownloadTPS("148", "155");
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Dictionary<string, string> DownloadProvinsiList()
        {
            Dictionary<string, string> PDict = new Dictionary<string, string>();
            WebClient client = new WebClient();
            var doc = new HtmlDocument();
            bool downloaded = false;
            while (!downloaded)
            {
                try
                {
                    doc.LoadHtml(client.DownloadString("http://pilpres2014.kpu.go.id/c1.php"));
                    downloaded = true;
                }
                catch
                {
                    downloaded = false;
                }
            }
            var provsel = doc.DocumentNode.Descendants("select");

            foreach (HtmlNode prov in provsel.Single().Descendants())
            {
                if (prov.Attributes.Contains("value"))
                {
                    if (!string.IsNullOrEmpty(prov.Attributes["value"].Value) && !string.IsNullOrWhiteSpace(prov.Attributes["value"].Value))
                    {
                        PDict.Add(prov.NextSibling.InnerText.Trim(), prov.Attributes["value"].Value);
                    }
                }
            }

            client.Dispose();
            return PDict;
        }

        private Dictionary<string, string> DownloadKab(string provID)
        {
            return DownloadList("0", provID);
        }

        private Dictionary<string, string> DownloadKec(string provID, string kabID)
        {
            return DownloadList(provID, kabID);
        }

        private Dictionary<string, string> DownloadDesa(string kabID, string kecID)
        {
            return DownloadList(kabID, kecID);
        }

        private Dictionary<string, string> DownloadList(string grandparent, string parent)
        {
            Dictionary<string, string> daftar = new Dictionary<string, string>();
            WebClient client = new WebClient();
            var doc = new HtmlDocument();

            bool downloaded = false;
            while (!downloaded)
            {
                try
                {
                    doc.LoadHtml(client.DownloadString("http://pilpres2014.kpu.go.id/c1.php?cmd=select&grandparent=" + grandparent + "&parent=" + parent));
                    downloaded = true;
                }
                catch
                {
                    downloaded = false;
                }
            }
            var provsel = doc.DocumentNode.Descendants("select");

            foreach (HtmlNode prov in provsel.Single().Descendants())
            {
                if (prov.Attributes.Contains("value"))
                {
                    if (!string.IsNullOrEmpty(prov.Attributes["value"].Value) && !string.IsNullOrWhiteSpace(prov.Attributes["value"].Value))
                    {
                        daftar.Add(prov.NextSibling.InnerText.Trim(), prov.Attributes["value"].Value);
                    }
                }
            }

            client.Dispose();
            return daftar;
        }
        /// <returns>Dictionary berbentuk nomor TPS => Link download.</returns>
        private Dictionary<string, string> DownloadTPS(string kecID, string desaID)
        {
            Dictionary<string, string> daftar = new Dictionary<string, string>();
            WebClient client = new WebClient();
            var doc = new HtmlDocument();
            bool downloaded = false;
            while (!downloaded)
            {
                try
                {
                    doc.LoadHtml(client.DownloadString("http://pilpres2014.kpu.go.id/c1.php?cmd=select&grandparent=" + kecID + "&parent=" + desaID));
                    downloaded = true;
                }
                catch
                {
                    downloaded = false;
                }
            }
            var table = doc.DocumentNode.Descendants("table").FirstOrDefault().Descendants("tr").ToList();

            for (int i = 3; i < table.Count() - 1; i++)
            {
                string IDTPS = table[i].Descendants("td").FirstOrDefault().InnerHtml.Trim();
                string downloadLink = "";
                try
                {
                    downloadLink = "http://pilpres2014.kpu.go.id" + table[i].Descendants("td").LastOrDefault().Descendants("a").FirstOrDefault().Attributes["href"].Value.Trim();
                }
                catch
                {
                    downloadLink = "";
                }
                Console.WriteLine("ID : " + IDTPS + " link : " + downloadLink);

                daftar.Add(IDTPS, downloadLink);
            }

            client.Dispose();
            return daftar;
        }

        private void TulisLog(string dir, string prov, string kab, string kec, string kel, string tps, bool success)
        {
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            if (File.Exists(dir + @"\log.txt"))
            {
                File.Delete(dir + @"\log.txt");
            }
            try
            {
                var mainFile = new StreamWriter(dir + @"\log.txt", true);
                string line = prov + "\t" + kab + "\t" + kec + "\t" + kel + "\tTPS-" + tps + " ";
                if (success)
                {
                    line += "sukses didownload";
                }
                else
                {
                    line += "gagal didownload";
                }
                mainFile.WriteLine(line);
                mainFile.Close();
            }
            catch (IOException)
            {
                throw;
            }
        }
    }
}
