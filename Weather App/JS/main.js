// Identifying the variables of the input elements
const cityInput = document.getElementById("cityInput");
const searchBtn = document.getElementById("searchBtn");
const apiKey = "e0df45531729e4accd910de89c3c69d6";
let lat, lon;
// The output element
const weatherResult = document.getElementById("weatherResult");

// Waiting for the action on the search button
searchBtn.addEventListener("click", () => {
  // Getting the coordinates
  const city = cityInput.value;
  if (city) {
    fetch(
      `http://api.openweathermap.org/geo/1.0/direct?q=${city}&limit=5&appid=${apiKey}`
    )
      .then((response) => response.json())
      .then((data) => {
        if (data.length > 0) {
          lat = data[0].lat;
          lon = data[0].lon;
          console.log(lat);
          console.log(lon);
          // Fetch weather data using the coordinates
          fetch(
            `https://api.open-meteo.com/v1/forecast?latitude=${lat}&longitude=${lon}&current_weather=true&temperature_2m=true&windspeed_10m=true&humidity_2m=true&precipitation=true&cloudcover=true&pressure_msl=true`
          )
            .then((response) => response.json())
            .then((data) => {
              const currentWeather = data.current_weather; // Extract current weather

              // Check if currentWeather exists
              if (currentWeather) {
                // Making sure that the animation happens after fetching data
                weatherResult.style.animation = "fadeInSlideUp 3s ease-out";

                // Display additional weather information
                weatherResult.innerHTML = `
                  <h2>Weather for ${city}</h2>
                  <p>Temperature: ${currentWeather.temperature}°C</p>
                  <p>Windspeed: ${currentWeather.windspeed} km/h</p>
                  <p>Humidity: ${currentWeather.humidity}%</p>
                  <p>Precipitation: ${data.precipitation} mm</p>
                  <p>Cloud Cover: ${data.cloudcover} %</p>
                  <p>Pressure: ${data.pressure_msl} hPa</p>
                `;
              } else {
                weatherResult.innerHTML = `<p>Failed to fetch current weather data.</p>`;
              }
            })
            .catch(() => {
              weatherResult.innerHTML = `<p>Failed to fetch weather data.</p>`;
            });
        } else {
          alert("Unable to fetch the coordinates. Check the city name.");
        }
      })
      .catch(() => alert("City not found!"));
  } else {
    weatherResult.innerHTML = "<p>Enter a city</p>";
  }
});
