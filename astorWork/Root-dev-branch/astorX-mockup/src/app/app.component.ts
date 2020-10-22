/// <reference path="../../node_modules/bingmaps/types/MicrosoftMaps/Microsoft.Maps.All.d.ts" />

import { Component, OnInit, AfterViewInit, AfterViewChecked } from '@angular/core';
import { MapPin } from './models/mapPin';
import { Chart } from 'chart.js';
import { ProjectCompletionTime } from './classes/project-completion-time';
@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  title = 'astorX-mockup';
  projectCompTime: ProjectCompletionTime;
  ngOnInit() {
    window.addEventListener("load", this.wndOnLoaded);
    this.loadBingMaps();
    this.projectDetails();
    this.GroupBarChartHorizontal();
    this.polarChart();
    this.lineChart();
    // this.bubbleChart();
    this.stackedBarChartHorizontal();
    this.stackedBarChartHorizontalFullWidth();
  }

  wndOnLoaded() {
    const INFO_DESC = `<style>
      table{ background-color: lightblue; border-radius: 5px; padding: 5px; }
      td {text-align:right; padding-right: 10px;}
      th {text-align:left;}
    </style>
    <table style="">
    <tbody>
      <tr>
        <td>Headcount</td>
        <th>5/10</th>
      </tr>
      <tr>
        <td>Supervisor Ratio</td>
        <th>1 : 5</th>
      </tr>
      <tr>
        <td>Safety Ratio</td>
        <th>5 : 20</th>
      </tr>
      <tr>
        <td>Non-compliance</td>
        <th>30%</th>
      </tr>
    </tbody>
  </table>`
    const BING_MAPS_KEY = 'AmpsWyTZXIT2m2Y6NAz0-Z7UZ7_M8NfCU4EZcBb-M-A7AO5gKl81tn_rqHvVKt8M';
    const data = [
      new MapPin(1.269920, 103.696895, 'Zone 1', 13, '', INFO_DESC),
      new MapPin(1.269202, 103.696205, 'Zone 2', 7, '', INFO_DESC),
      new MapPin(1.269490, 103.696464, 'Zone 3', 22, '', INFO_DESC)
    ];
    if (typeof Microsoft !== 'undefined') {
      var map = new Microsoft.Maps.Map(document.getElementById('myMap'), {
        credentials: BING_MAPS_KEY,
        center: new Microsoft.Maps.Location(1.269799, 103.696695),
        zoom: 18,
        mapTypeId: Microsoft.Maps.MapTypeId.aerial,
        enableClickableLogo: false,
        showMapTypeSelector: false,
        showDashboard: false
      });

      let textOffsetPoint = new Microsoft.Maps.Point(0, 10);
      let infoOffsetPoint = new Microsoft.Maps.Point(-80, 48);
      // Add push-pins
      for (let item of data) {
        let loc = new Microsoft.Maps.Location(item.latitude, item.longitude);
        let pushpin = new Microsoft.Maps.Pushpin(loc, {
          icon: item.pinIcon,
          textOffset: textOffsetPoint,
          text: String(item.count),
          title: item.title,
          color: item.pinColor
        });

        let infobox = new Microsoft.Maps.Infobox(loc, {
          htmlContent: item.infoDesc,
          visible: false,
          offset: infoOffsetPoint
        });
        infobox.setMap(map);

        Microsoft.Maps.Events.addHandler(pushpin, 'mouseover', () => {
          infobox.setOptions({ visible: true });
        });
        Microsoft.Maps.Events.addHandler(pushpin, 'mouseout', () => {
          infobox.setOptions({ visible: false });
        });
        map.entities.push(pushpin);
      }
    }
  }
  loadBingMaps() {
    const scriptElement = document.createElement('script');
    scriptElement.src = 'https://www.bing.com/api/maps/mapcontrol';
    scriptElement.type = 'text/javascript';
    var x = document.getElementsByTagName('head')[0];
    x.appendChild(scriptElement);
  }
  
  polarChart() {
    new Chart(document.getElementById("polar-chart"), {
      type: 'polarArea',
      data: {
        labels: ["HCU", "HFA", "PU1", "PU2", "PU3", "Tank219", "Tank134"],
        datasets: [
          {
            label: "Population (millions)",
            backgroundColor: ["#3e95cd", "#8e5ea2", "#3cba9f", "#e8c3b9", "#c45850", "#7b7777", "#fce80f"],
            data: [83, 45, 50, 40, 67, 70, 120]
          }
        ]
      },
      options: {
        title: {
          display: true,
          text: 'Headcount by Zone'
        },
        legend: {
          display: true,
          labels: {
              fontSize: 10,
              boxWidth: 10,
              padding: 5
          }
      },
      }
    });
  }

  GroupBarChartHorizontal() {
    new Chart(document.getElementById("bar-chart-grouped"), {
      type: 'horizontalBar',
      data: {
        labels: ["Zone 1", "Zone 2", "Zone 3", "Zone 4", "Zone 5"],
        datasets: [
          {
            label: "Actual Headcount",
            backgroundColor: "#3e95cd",
            data: [90, 65, 70, 90, 50]
          }, {
            label: "Max Headcount",
            backgroundColor: "darkorange",
            data: [110, 80, 60, 100, 40]
          }
        ]
      },
      options: {
        title: {
          display: true,
          text: 'Safety Ratio'
        },
        legend: {
          display: true,
          labels: {
              fontSize: 10,
              boxWidth: 10,
              padding: 5
          }
      },
        scales: {
          xAxes: [{
            scaleLabel: {
              display: true,
              labelString: "Date",
            },
            ticks: {
              beginAtZero: true
            }
          }]
        },
        plugins: {
                      datalabels: {
                          color: 'white',
                          display: function(context) {
                              console.log("Algo: "+context);
                              return context.dataset.data[context.dataIndex] > 15;
                          },
                          font: {
                              weight: 'bold'
                          },
                          formatter: function(value, context) {
                          return context.dataIndex + ': ' + Math.round(value*100) + '%';
                      }
                      }
                  }
      }
    });
  }

  stackedBarChartHorizontal() {
    new Chart(document.getElementById("stacked-bar-chart-grouped"), {
      type: 'horizontalBar',
      data: {
        labels: ["Zone 5", "Zone 4", "Zone 3", "Zone 2", "Zone 1"],
        datasets: [
          {
            label: "Workforce",
            backgroundColor: "#3e95cd",
            data: [20, 60, 55, 100, 20]
          }, {
            label: "Supervisor",
            backgroundColor: "darkorange",
            data: [3, 2, 3, 2, 2]
          }
        ]
      },
      options: {
        title: {
          display: true,
          text: 'Supervisor Ratio'
        },
        legend: {
          display: true,
          labels: {
              fontSize: 10,
              boxWidth: 10,
              padding: 5
          }
      },
        scales: {
          xAxes: [{
            stacked: true,
            scaleLabel: {
              display: true,
              labelString: "Date",
            },
            ticks: {
              beginAtZero: true,
              fontSize:10
            }
          }],
          yAxes: [{
            stacked: true,
            ticks: {
              fontSize:10
            }
          }]
        }
      }
    });
  }

  stackedBarChartHorizontalFullWidth() {
    new Chart(document.getElementById("stacked-bar-chart"), {
      type: 'horizontalBar',
      data: {
        labels: ["Zone 1", "Zone 2", "Zone 3"],
        datasets: [
          {
            label: "Certificate",
            backgroundColor: "#0dbbaa",
            data: [14.29, 23.68, 18.60]
          }, {
            label: "Workforce",
            backgroundColor: "black",
            data: [57.14, 5.27, 34.88]
          },
          {
            label: "Zone",
            backgroundColor: "red",
            data: [28.57, 71.05, 46.51]
          }
        ]
      },
      options: {
        title: {
          display: true,
          text: 'Non-Compliance Summary'
        },
        legend: {
          display: true,
          labels: {
              fontSize: 10,
              boxWidth: 10,
              padding: 5
          }
      },
        scales: {
          xAxes: [{
            stacked: true,
            scaleLabel: {
              display: true,
              labelString: "Date",
            },
            ticks: {
              beginAtZero: true
            }
          }],
          yAxes: [{
            stacked: true,
            ticks: {
              fontSize: 10
            }
          }]
        }
      }
    });
  }

  lineChart() {
    new Chart(document.getElementById("line-chart"), {
      type: 'line',
      data: {
        labels: ['Zone 1', 'Zone 2', 'Zone 3', 'Zone 4', 'Zone 5'],
        datasets: [{
          data: [390, 260, 340, 210, 205],
          label: "Actual",
          borderColor: "#3e95cd",
          fill: true
        }, {
          data: [400, 270, 360, 200, 205],
          label: "Planned",
          borderColor: "#8e5ea2",
          fill: true
        }
        ]
      },
      options: {
        title: {
          display: true,
          text: 'Actual and Planned by Zone'
        },
        legend: {
          display: true,
          labels: {
              fontSize: 10,
              boxWidth: 10,
              padding: 5
          }
      },
      plugins: {
                    datalabels: {
                        color: 'white',
                        display: function(context) {
                            console.log("Algo: "+context);
                            return context.dataset.data[context.dataIndex] > 15;
                        },
                        font: {
                            weight: 'bold'
                        },
                        formatter: function(value, context) {
                        return context.dataIndex + ': ' + Math.round(value*100) + '%';
                    }
                    }
                }
      }
    });
  }

  bubbleChart() {
    new Chart(document.getElementById("bubble-chart"), {
      type: 'bubble',
      data: {
        labels: "HeadCount",
        datasets: [
          {
            label: ["HCU"],
            backgroundColor: "rgba(255,221,50,0.2)",
            borderColor: "rgba(255,221,50,1)",
            data: [{
              x: 5,
              y: 83,
              r: 40
            }]
          }, {
            label: ["HFA"],
            backgroundColor: "rgba(60,186,159,0.2)",
            borderColor: "rgba(60,186,159,1)",
            data: [{
              x: 10,
              y: 45,
              r: 25
            }]
          }, {
            label: ["PU1"],
            backgroundColor: "rgba(0,0,0,0.2)",
            borderColor: "#000",
            data: [{
              x: 15,
              y: 50,
              r: 30
            }]
          }, {
            label: ["PU2"],
            backgroundColor: "rgba(193,46,12,0.2)",
            borderColor: "rgba(193,46,12,1)",
            data: [{
              x: 20,
              y: 40,
              r: 20
            }]
          }
        ]
      },
      options: {
        title: {
          display: true,
          text: 'Headcount by Zone'
        }, scales: {
          yAxes: [{
            scaleLabel: {
              display: true,
              labelString: "Head Count"
            }
          }],
          xAxes: [{
            scaleLabel: {
              display: true,
              labelString: "Zone"
            }
          }]
        }
      }
    });
  }

  projectDetails() {
    this.projectCompTime = new ProjectCompletionTime();
    this.projectCompTime.years = 0;
    this.projectCompTime.months = 3;
    this.projectCompTime.days = 0;
    this.projectCompTime.plannedHeadCount = 550;
    this.projectCompTime.actualHeadCount = 450;
  }
}
