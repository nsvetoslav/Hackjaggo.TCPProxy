using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;
using System.Xml.Linq;
using System.Globalization;

namespace Hackjaggo.Proxy
{
    public partial class GoogleMapsForm : Form
    {
        private double Longtitude { get; set; }
        private double Latitude { get; set; }
        private string GoogleAPIKey { get; set; }


        public GoogleMapsForm(double longtitude, double latitude, string apiKey)
        {
            Longtitude = longtitude;
            Latitude = latitude;
            GoogleAPIKey = apiKey;

            InitializeComponent();
            InitializeAsync();
        }

        private async void InitializeAsync()
        {
            await webView21.EnsureCoreWebView2Async(null);
            string htmlContent = $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <meta name='viewport' content='initial-scale=1.0, user-scalable=no' />
                    <style type='text/css'>
                        html, body {{
                            height: 100%;
                            margin: 0;
                            padding: 0;
                        }}
                        #map-canvas {{
                            height: 100%;
                        }}
                    </style>
                    <script type='text/javascript' src='https://maps.googleapis.com/maps/api/js?key={GoogleAPIKey}&sensor=false'></script>
                    <script type='text/javascript'>
                        function initialize() {{
                            var myLatlng = {{ lat: {Latitude.ToString(CultureInfo.InvariantCulture)}, lng: {Longtitude.ToString(CultureInfo.InvariantCulture)} }};
                            var mapOptions = {{
                                center: myLatlng,
                                zoom: 15
                            }};
                            var map = new google.maps.Map(document.getElementById('map-canvas'), mapOptions);

                            var marker = new google.maps.Marker({{
                                position: myLatlng,
                                map: map,
                                title: 'Here it is!'
                            }});
                        }}
                        window.onload = initialize;
                    </script>
                </head>
                <body>
                    <div id='map-canvas'></div>
                </body>
                </html>";
            webView21.NavigateToString(htmlContent);
        }
    }
}

