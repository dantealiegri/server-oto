using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;
using System.IO;

namespace OtoServer.Omaha
{
    public class Testing
    {

        public static void Serialize()
        {
            XmlSerializer client_serialize = new XmlSerializer( typeof(V3.Request));
            XmlSerializer server_serialize = new XmlSerializer( typeof(V3.Result));
            StringWriter ssw = new StringWriter();
            StringWriter csw = new StringWriter();
            StringWriter sdsw = new StringWriter();
            StringWriter cdsw = new StringWriter();

            V3.Request sample_request = new V3.Request {
                protocol = "3.0",
                version = "1.3.23.0",
                ismachine = "0",
                sessionid = "{5FAD27D4-6BFA-4daa-A1B3-5A1F821FEE0F}",
                userid = "{D0BBD725-742D-44ae-8D46-0231E881D58E}",
                installsource="scheduler",
                testsource="ossdev",
                requestid="{C8F6EDF3-B623-4ee6-B2DA-1D08A0B4C665}",
                os = new V3.OSInfo {
                    platform = "win",
                    version = "6.1",
                    sp = "",
                    arch="x64"
                },
                apps = new List<V3.AppInfoRequest>( new V3.AppInfoRequest[] {
                    new V3.AppInfoRequest {
                        appid = "{430FD4D0-B729-4F61-AA34-91526481799D}",
                        version = "1.3.23.0",
                        nextversion = "",
                        lang = "en",
                        brand = "GGLS",
                        client = "someclientid",
                        installage = 39,
                        updatecheck = new V3.UpdateCheck(),
                        ping = new V3.PingRequest { r = "1" }
                    },
                    new V3.AppInfoRequest {
                        appid = "{D0AB2EBC-931B-4013-9FEB-C9C4C2225C8C}",
                        version = "2.2.2.0",
                        nextversion = "",
                        lang = "en",
                        brand = "GGLS",
                        client = "",
                        installage = 6,
                        updatecheck = new V3.UpdateCheck(),
                        ping = new V3.PingRequest { r = "1" }
                    }
                }
                )
            };

            V3.Result sample_result = new V3.Result{
                protocol = "3.0",
                server = "prod",
                daystart = new V3.DayStart { elapsed_seconds = 56508 },
                app_results = new List<V3.AppInfoResult>( new V3.AppInfoResult[] {
                    new V3.AppInfoResult {
                        appid = "{430FD4D0-B729-4F61-AA34-91526481799D}",
                        status = "ok",
                        updatecheck = new V3.UpdateResult { status = "noupdate" },
                        ping = new V3.PingResult { status = "ok" }
                    },

                    new V3.AppInfoResult {
                        appid = "{D0AB2EBC-931B-4013-9FEB-C9C4C2225C8C}",
                        status = "ok",
                        updatecheck = new V3.UpdateResult {
                            status = "ok",
                            urls = new List<V3.UpdateUrl>( new V3.UpdateUrl[] {
                                new V3.UpdateUrl {
                                    codebase = "http://cache.pack.google.com/edgedl/chrome/install/782.112/"
                                }
                            }),
                            manifest = new V3.UpdateManifest {
                                version = "13.0.782.112",
                                updated_packages = new List<V3.UpdatePackage>( new V3.UpdatePackage[] {
                                    new V3.UpdatePackage {
                                        hash = "VXriGUVI0TNqfLlU02vBel4Q3Zo=",
                                        name="chrome_installer.exe",
                                        required="true",
                                        size=23963192
                                    }
                                }),
                                update_actions = new List<V3.UpdateAction>( new V3.UpdateAction[] {
                                    new V3.UpdateAction {
                                        arguments ="--do-not-launch-chrome",
                                        on_event = "install",
                                        run="chrome_installer.exe"
                                    },
                                    new V3.UpdateAction {
                                        on_event = "postinstall",
                                        version = "13.0.782.112",
                                        onsuccess = "exitsilentlyonlaunchcmd"
                                    }
                                })
                            }
                        }
                    }
                })
            };

            V3.Request sample_data_request = new V3.Request {
                protocol = "3.0",
                version = "1.3.23.0",
                ismachine = "0",
                sessionid = "{5FAD27D4-6BFA-4daa-A1B3-5A1F821FEE0F}",
                userid = "{D0BBD725-742D-44ae-8D46-0231E881D58E}",
                installsource="scheduler",
                testsource="ossdev",
                requestid="{C8F6EDF3-B623-4ee6-B2DA-1D08A0B4C665}",
                os = new V3.OSInfo {
                    platform = "win",
                    version = "6.1",
                    sp = "",
                    arch="x64"
                },
                apps = new List<V3.AppInfoRequest>( new V3.AppInfoRequest[] {
                    new V3.AppInfoRequest {
                        appid = "{430FD4D0-B729-4F61-AA34-91526481799D}",
                        version = "1.3.23.0",
                        nextversion = "",
                        lang = "en",
                        brand = "GGLS",
                        client = "someclientid",
                        installage = 39,
                        updatecheck = new V3.UpdateCheck(),
                        ping = new V3.PingRequest { r = "1" },
                        data = new List<V3.DataRequest>( new V3.DataRequest[] {
                            new V3.DataRequest {
                                name = "install",
                                index = "verboselogging"
                            },
                            new V3.DataRequest {
                                name="untrusted",
                                data="Some untrusted data"
                            }
                        })
                    }
                }
                )
            };
            V3.Result sample_data_result = new V3.Result{
                protocol = "3.0",
                server = "prod",
                daystart = new V3.DayStart { elapsed_seconds = 56508 },
                app_results = new List<V3.AppInfoResult>( new V3.AppInfoResult[] {
                    new V3.AppInfoResult {
                        appid = "{430FD4D0-B729-4F61-AA34-91526481799D}",
                        status = "ok",
                        updatecheck = new V3.UpdateResult { status = "noupdate" },
                        ping = new V3.PingResult { status = "ok" },
                        data_results = new List<V3.DataResult>( new V3.DataResult[] {
                            new V3.DataResult {
                                index = "verboselogging",
                                name = "install",
                                status = "ok",
                            data = "app-specific values go here"
                            },
                            new V3.DataResult {
                                    name = "untrusted",
                                    status = "ok"
                                }
                        })
                    },

                })
            };
                                



            client_serialize.Serialize(csw, sample_request);
            Console.WriteLine("Client is " + csw.ToString() );
            server_serialize.Serialize(ssw, sample_result);
            Console.WriteLine("Server is " + ssw.ToString() );

            client_serialize.Serialize(cdsw, sample_data_request);
            server_serialize.Serialize(sdsw, sample_data_result);
        }
    }
}