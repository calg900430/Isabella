var ctx = document.getElementById("myChart").getContext("2d");
var myChart = new Chart(ctx, {
  type: "line",
  data: {
     labels: [
      "Monday",
      "Tuesday",
      "Wednesday",
      "Thursday",
      "Friday",
      "Saturday",
      "Sunday",
    ],
    datasets: [
          {
        label: "work load",
        data: [2, 9, 3, 17, 6, 3, 7],
        backgroundColor: "rgba(60,141,188,0.9)",
  },
          {
        label: "free hours",
    data: [2, 2, 5, 5, 2, 1, 10],
    backgroundColor: "rgba(155,153,10,0.6)",
  },
],
},
});
