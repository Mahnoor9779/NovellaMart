// This function is called from Home.razor onAfterRender
export function initFilterAccordions() {
    const filterTitles = document.querySelectorAll('.filter-title');

    filterTitles.forEach(title => {
        title.addEventListener('click', function () {
            // Find the parent group
            const parentGroup = this.parentElement;

            // Toggle the 'active' class on the parent group
            // The CSS handles showing/hiding the .filter-content based on this class
            parentGroup.classList.toggle('active');

            // Optional: Rotate the icon
            const icon = this.querySelector('i');
            if (icon) {
                if (parentGroup.classList.contains('active')) {
                    icon.classList.replace('fa-chevron-down', 'fa-chevron-up');
                } else {
                    icon.classList.replace('fa-chevron-up', 'fa-chevron-down');
                }
            }
        });
    });

    // Mobile Filter Sidebar Toggle (Optional implementation)
    const toggleBtn = document.getElementById('toggleFiltersBtn');
    const sidebar = document.querySelector('.shop-sidebar');
    const closeBtn = document.querySelector('.btn-close-filters');

    if (toggleBtn && sidebar) {
        toggleBtn.addEventListener('click', () => {
            sidebar.classList.add('show');
        });
    }

    if (closeBtn && sidebar) {
        closeBtn.addEventListener('click', () => {
            sidebar.classList.remove('show');
        });
    }
}