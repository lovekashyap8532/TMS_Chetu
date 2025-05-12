document.addEventListener("DOMContentLoaded", function () {
    let searchBox = document.getElementById("searchContainer");
    let searchInput = document.getElementById("searchInput");
    let showAllBtn = document.getElementById("showAllBtn");

    // Ensure the table exists before proceeding
    let taskTable = document.getElementById("taskTable");
    if (!taskTable) return; // Stop execution if table is not found

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
            if (searchBox) searchBox.style.display = "none";
            if (searchInput) searchInput.value = "";

            filterTable(type, value);
            if (showAllBtn) showAllBtn.style.display = "block";
        });
    });

    // Show search bar when clicking "Search"
    let searchTrigger = document.getElementById("searchTrigger");
    if (searchTrigger) {
        searchTrigger.addEventListener("click", function (event) {
            event.preventDefault();
            if (searchBox) {
                searchBox.style.display = "block";
                searchInput.focus();
            }
        });
    }

    // Search filter function
    if (searchInput) {
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

            if (showAllBtn) showAllBtn.style.display = found ? "block" : "none";
        });
    }

    // Filter function for status & priority
    function filterTable(type, value) {
        let rows = document.querySelectorAll("#taskTable tbody tr");
        let found = false;

        rows.forEach(row => {
            let statusCell = row.querySelector("td:nth-child(6) button");
            let priorityCell = row.querySelector("td:nth-child(7) button");

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

        if (showAllBtn) showAllBtn.style.display = found ? "block" : "none";
    }

    // Show all data when "Show All Data" is clicked
    if (showAllBtn) {
        showAllBtn.addEventListener("click", function () {
            document.querySelectorAll("#taskTable tbody tr").forEach(row => row.style.display = "");
            if (searchBox) searchBox.style.display = "none";
            if (searchInput) searchInput.value = "";
            showAllBtn.style.display = "none";
        });
    }
});
