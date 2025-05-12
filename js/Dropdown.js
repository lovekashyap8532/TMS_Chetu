    document.addEventListener("DOMContentLoaded", function () {
        let searchBox = document.getElementById("searchContainer");
    let searchInput = document.getElementById("searchInput");
    let showAllBtn = document.getElementById("showAllBtn");

        // Handle dropdown hover for submenus
        document.querySelectorAll(".dropdown-submenu").forEach((submenu) => {
        submenu.addEventListener("mouseover", function () {
            let menu = this.querySelector(".dropdown-menu");
            if (menu) menu.style.display = "block";
        });

    submenu.addEventListener("mouseleave", function () {
        let menu = this.querySelector(".dropdown-menu");
    if (menu) menu.style.display = "none";
            });
        });

        // Handle dropdown item clicks (status, priority)
        document.querySelectorAll(".filter-option").forEach((item) => {
        item.addEventListener("click", function (event) {
            event.preventDefault();
            let type = this.dataset.type;
            let value = this.textContent.trim();

            // Hide search bar when selecting a filter
            searchBox.style.display = "none";
            searchInput.value = "";

            filterTable(type, value);
            showAllBtn.style.display = "block"; // Show "Show All Data" button
        });
        });

    // Show search bar when clicking "Search"
    document.getElementById("searchTrigger").addEventListener("click", function (event) {
        event.preventDefault();
    searchBox.style.display = "block";
    searchInput.focus();
        });

    // Search filter function
    searchInput.addEventListener("input", function () {
        let searchValue = this.value.toLowerCase().trim();
    let rows = document.querySelectorAll("#taskTable tbody tr");
    let found = false;

            rows.forEach(row => {
        let titleCell = row.querySelector("td:nth-child(2)"); // Title column
    let assignedToCell = row.querySelector("td:nth-child(8)"); // AssignedTo column

    let title = titleCell ? titleCell.textContent.toLowerCase().trim() : "";
    let assignedTo = assignedToCell ? assignedToCell.textContent.toLowerCase().trim() : "";

    if (title.includes(searchValue) || assignedTo.includes(searchValue)) {
        row.style.display = "";
    found = true;
                } else {
        row.style.display = "none";
                }
            });

    showAllBtn.style.display = found ? "block" : "none"; // Show button if results found
        });

    // Filter function for status & priority
    function filterTable(type, value) {
        let rows = document.querySelectorAll("#taskTable tbody tr");
    let found = false;

            rows.forEach(row => {
        let statusCell = row.querySelector("td:nth-child(6) a");
    let priorityCell = row.querySelector("td:nth-child(7) a");

    let status = statusCell ? statusCell.textContent.trim() : "";
    let priority = priorityCell ? priorityCell.textContent.trim() : "";

    let statusMatch = type === "status" ? status === value : true;
    let priorityMatch = type === "priority" ? priority === value : true;

    if (statusMatch && priorityMatch) {
        row.style.display = "";
    found = true;
                } else {
        row.style.display = "none";
                }
            });

    showAllBtn.style.display = found ? "block" : "none"; // Show button if results found
        }

    // Show all data when "Show All Data" is clicked
    showAllBtn.addEventListener("click", function () {
        document.querySelectorAll("#taskTable tbody tr").forEach(row => row.style.display = "");
    searchBox.style.display = "none";
    searchInput.value = "";
    showAllBtn.style.display = "none"; // Hide the button
        });
    });


