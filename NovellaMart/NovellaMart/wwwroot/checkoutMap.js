let map;
let marker;

// 1. Initialize the Map
export function initializeMap(elementId) {
    // Default view (World or specific region)
    // Coords: [30.3753, 69.3451] is roughly central Pakistan, changeable to any default.
    map = L.map(elementId).setView([30.3753, 69.3451], 5);

    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        attribution: '&copy; OpenStreetMap contributors'
    }).addTo(map);
}

// 2. Update Map based on City Name
export async function updateMapLocation(cityName) {
    if (!cityName) return;

    try {
        // Use free Nominatim API for Geocoding
        const response = await fetch(`https://nominatim.openstreetmap.org/search?format=json&q=${cityName}`);
        const data = await response.json();

        if (data && data.length > 0) {
            const lat = data[0].lat;
            const lon = data[0].lon;

            // Move map
            map.setView([lat, lon], 12);

            // Update/Add Marker
            if (marker) {
                marker.setLatLng([lat, lon]);
            } else {
                marker = L.marker([lat, lon]).addTo(map);
            }

            // Add a popup
            marker.bindPopup(`<b>${cityName}</b><br>Delivering here!`).openPopup();
        }
    } catch (error) {
        console.error("Error finding location:", error);
    }
}