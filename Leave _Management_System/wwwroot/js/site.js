// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Keep validation messages visible until the problematic field is corrected
document.addEventListener('DOMContentLoaded', function () {
    // Find the Name input field
    const nameInput = document.getElementById('NameInput');

    // Find validation summary alert for "Name already exists" errors
    const validationSummary = document.querySelector('div[asp-validation-summary]');

    if (nameInput && validationSummary) {
        // Store the original validation error message
        const originalErrorMessage = validationSummary.textContent.trim();
        const hasNameError = originalErrorMessage.includes('already exists');

        // If there's a name validation error, watch for changes to the name field
        if (hasNameError) {
            // Add close button once
            if (!validationSummary.querySelector('.btn-close')) {
                const closeButton = document.createElement('button');
                closeButton.type = 'button';
                closeButton.className = 'btn-close';
                closeButton.setAttribute('data-bs-dismiss', 'alert');
                closeButton.setAttribute('aria-label', 'Close');
                validationSummary.appendChild(closeButton);
            }

            // Listen to name input changes
            nameInput.addEventListener('input', function () {
                // When user types in the name field, hide the error message temporarily
                // to indicate they're in the process of fixing it
                validationSummary.style.opacity = '0.5';
                validationSummary.style.transition = 'opacity 0.3s ease';
            });

            // Reset opacity when user stops typing (to show they need to submit)
            let typingTimer;
            nameInput.addEventListener('input', function () {
                clearTimeout(typingTimer);
                validationSummary.style.opacity = '1';

                typingTimer = setTimeout(function () {
                    // After 2 seconds of no typing, bring message back to full opacity
                    validationSummary.style.opacity = '1';
                }, 2000);
            });

            // Prevent the validation alert from being dismissed by Bootstrap's auto-dismiss
            // by removing the fade and show classes if they try to auto-dismiss
            const observer = new MutationObserver(function (mutations) {
                mutations.forEach(function (mutation) {
                    if (mutation.type === 'class') {
                        // Keep it visible - re-add the show class if it's removed
                        if (!validationSummary.classList.contains('show')) {
                            validationSummary.classList.add('show');
                        }
                    }
                });
            });

            observer.observe(validationSummary, { attributes: true, attributeFilter: ['class'] });
        }
    }
});

