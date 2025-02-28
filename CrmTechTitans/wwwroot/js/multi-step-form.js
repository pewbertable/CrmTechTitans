/**
 * Multi-step Form Functionality
 * For CrmTechTitans Member Creation
 */

// Declare jQuery variable if it's not already defined globally.
const $ = window.$ || jQuery;

document.addEventListener("DOMContentLoaded", () => {
    const steps = document.querySelectorAll(".form-step");
    const nextButtons = document.querySelectorAll(".next-step");
    const prevButtons = document.querySelectorAll(".prev-step");
    const progressBar = document.getElementById("progress-bar");
    const stepIndicators = document.querySelectorAll(".step-indicator");
    const form = document.getElementById("multiStepForm");

    let currentStep = 0;
    const totalSteps = steps.length;

    // Initialize form validation
    if (typeof $.validator !== "undefined" && typeof $.validator.unobtrusive !== "undefined") {
        $.validator.unobtrusive.parse(form);
    }

    // Show specific step and update indicators
    function showStep(step) {
        steps.forEach((s, index) => {
            s.classList.toggle("active", index === step);
        });

        currentStep = step;

        // Update progress bar
        const percent = (currentStep / (totalSteps - 1)) * 100;
        progressBar.style.width = `${percent}%`;

        // Update step indicators
        stepIndicators.forEach((indicator, index) => {
            indicator.classList.toggle("active", index === step);
            indicator.classList.toggle("completed", index < step);
        });
    }

    // Validate form fields for current step
    function validateCurrentStep() {
        const currentStepElement = steps[currentStep];
        let isValid = true;

        // Validate required fields
        const requiredFields = currentStepElement.querySelectorAll(".required, .required-address, .required-contact");

        requiredFields.forEach((field) => {
            let errorSpan = field.closest(".form-group")?.querySelector(".field-validation-error");

            if (!field.value.trim()) {
                isValid = false;
                field.classList.add("invalid");

                // Show error message
                if (errorSpan) {
                    errorSpan.style.display = "block";
                }
            } else {
                field.classList.remove("invalid");

                // Hide error message if input is valid
                if (errorSpan) {
                    errorSpan.style.display = "none";
                }
            }
        });

        // Step 1 Specific: Validate membership type selection
        if (currentStepElement.id === "step1") {
            const membershipChecked = document.querySelectorAll(".membership-type:checked").length > 0;
            const membershipError = document.querySelector(".membership-error");

            if (membershipError) {
                membershipError.style.display = membershipChecked ? "none" : "block";
            }

            isValid = isValid && membershipChecked;
        }

        return isValid;
    }




    // Navigate to next step
    nextButtons.forEach((button) => {
        button.addEventListener("click", () => {
            if (validateCurrentStep() && currentStep < steps.length - 1) {
                showStep(currentStep + 1);
            }
        });
    });

    // Navigate to previous step
    prevButtons.forEach((button) => {
        button.addEventListener("click", () => {
            if (currentStep > 0) {
                showStep(currentStep - 1);
            }
        });
    });

    // Image preview functionality
    function previewImage(input, previewId) {
        const preview = document.getElementById(previewId);
        if (input.files && input.files[0]) {
            const reader = new FileReader();
            reader.onload = (e) => {
                preview.src = e.target.result;
                preview.style.display = "block";
            };
            reader.readAsDataURL(input.files[0]);
        }
    }

    window.previewContactImage = (input, index) => {
        previewImage(input, `contact-preview-${index}`);
    };

    // Add Address functionality
    document.getElementById("add-address").addEventListener("click", () => {
        const addressesContainer = document.getElementById("addresses-container");
        const nextIndex = document.querySelectorAll(".address-form").length;

        const newAddressForm = document.createElement("div");
        newAddressForm.className = "address-form form-card";
        newAddressForm.dataset.addressIndex = nextIndex;
        newAddressForm.innerHTML = `
        <div class="form-card-header">
            <h3>Address ${nextIndex + 1}</h3>
            <button type="button" class="btn btn-danger remove-address">
                <i class="fas fa-trash"></i> Remove
            </button>
        </div>
        <div class="form-card-body">
            <div class="form-row">
                <div class="form-group half-width">
                    <label>Street <span class="required-asterisk">*</span></label>
                    <div class="input-group">
                        <span class="input-icon"><i class="fas fa-map-marker-alt"></i></span>
                        <input name="Addresses[${nextIndex}].Street" class="form-input required-address" placeholder="Enter Street Address" />
                    </div>
                </div>
                <div class="form-group half-width">
                    <label>City <span class="required-asterisk">*</span></label>
                    <div class="input-group">
                        <span class="input-icon"><i class="fas fa-city"></i></span>
                        <input name="Addresses[${nextIndex}].City" class="form-input required-address" placeholder="Enter City" />
                    </div>
                </div>
            </div>
            <div class="form-row">
                <div class="form-group third-width">
                    <label>Province <span class="required-asterisk">*</span></label>
                    <select name="Addresses[${nextIndex}].Province" class="form-select required-address">
                        <option value="">Select Province</option>
                        ${Array.from(document.querySelector('select[name="Addresses[0].Province"]').options)
                .map((opt) => `<option value="${opt.value}">${opt.text}</option>`)
                .join("")}
                    </select>
                </div>
                <div class="form-group third-width">
                    <label>Postal Code</label>
                    <div class="input-group">
                        <span class="input-icon"><i class="fas fa-mail-bulk"></i></span>
                        <input name="Addresses[${nextIndex}].PostalCode" class="form-input" placeholder="Enter Postal Code" />
                    </div>
                </div>
                <div class="form-group third-width">
                    <label>Address Type</label>
                    <select name="Addresses[${nextIndex}].AddressType" class="form-select">
                        <option value="">Select Address Type</option>
                        ${Array.from(document.querySelector('select[name="Addresses[0].AddressType"]').options)
                .map((opt) => `<option value="${opt.value}">${opt.text}</option>`)
                .join("")}
                    </select>
                </div>
            </div>
        </div>
    `;

        addressesContainer.appendChild(newAddressForm);

        // Add remove handler
        newAddressForm.querySelector(".remove-address").addEventListener("click", function () {
            if (document.querySelectorAll(".address-form").length > 1) {
                this.closest(".address-form").remove();
            }
        });

        // Re-initialize form validation for the new address form
        if (typeof $.validator !== "undefined" && typeof $.validator.unobtrusive !== "undefined") {
            $.validator.unobtrusive.parse(newAddressForm);
        }
    });

    // Add Contact functionality
    document.getElementById("add-contact").addEventListener("click", () => {
        const contactsContainer = document.getElementById("contacts-container");
        const nextIndex = document.querySelectorAll(".contact-form").length;

        const newContactForm = document.createElement("div");
        newContactForm.className = "contact-form form-card";
        newContactForm.innerHTML = `
            <div class="form-card-header">
                <h3>Contact ${nextIndex + 1}</h3>
                <button type="button" class="btn btn-danger remove-contact">
                    <i class="fas fa-trash"></i> Remove
                </button>
            </div>
            <div class="form-card-body">
                <div class="form-row">
                    <div class="form-group half-width">
                        <label>First Name <span class="required-asterisk">*</span></label>
                        <div class="input-group">
                            <span class="input-icon"><i class="fas fa-user"></i></span>
                            <input name="Contacts[${nextIndex}].FirstName" class="form-input required-contact" placeholder="Enter first name" />
                        </div>
                    </div>
                    <div class="form-group half-width">
                        <label>Last Name</label>
                        <div class="input-group">
                            <span class="input-icon"><i class="fas fa-user"></i></span>
                            <input name="Contacts[${nextIndex}].LastName" class="form-input" placeholder="Enter last name" />
                        </div>
                    </div>
                </div>
                <div class="form-row">
                    <div class="form-group half-width">
                        <label>Email</label>
                        <div class="input-group">
                            <span class="input-icon"><i class="fas fa-envelope"></i></span>
                            <input name="Contacts[${nextIndex}].Email" class="form-input" placeholder="Enter email" type="email" />
                        </div>
                    </div>
                    <div class="form-group half-width">
                        <label>Phone <span class="required-asterisk">*</span></label>
                        <div class="input-group">
                            <span class="input-icon"><i class="fas fa-phone"></i></span>
                            <input name="Contacts[${nextIndex}].Phone" class="form-input required-contact" placeholder="Enter phone number" />
                        </div>
                    </div>
                </div>
                <div class="form-row">
                    <div class="form-group half-width">
                        <label>Contact Photo</label>
                        <div class="input-group">
                            <span class="input-icon"><i class="fas fa-camera"></i></span>
                            <input type="file" name="ContactPicture" class="form-input" accept="image/*" onchange="previewContactImage(this, ${nextIndex})" />
                        </div>
                        <div class="image-preview">
                            <img class="preview-image contact-preview" id="contact-preview-${nextIndex}" src="#" alt="Contact photo preview" />
                        </div>
                    </div>
                    <div class="form-group half-width">
                        <label>Contact Type</label>
                        <select name="Contacts[${nextIndex}].ContactType" class="form-select">
                            <option value="">Select Contact Type</option>
                            ${Array.from(document.querySelector('select[name="Contacts[0].ContactType"]').options)
                .map((opt) => `<option value="${opt.value}">${opt.text}</option>`)
                .join("")}
                        </select>
                    </div>
                </div>
            </div>
        `;

        contactsContainer.appendChild(newContactForm);
        if (typeof $.validator !== "undefined" && typeof $.validator.unobtrusive !== "undefined") {
            $.validator.unobtrusive.parse(newContactForm);
        }

        // Add remove handler
        newContactForm.querySelector(".remove-contact").addEventListener("click", () => {
            newContactForm.remove();
        });
    });

    // Form submission validation
    form.addEventListener("submit", (e) => {
        let isValid = true;
        for (let i = 0; i < steps.length; i++) {
            showStep(i);
            if (!validateCurrentStep()) {
                isValid = false;
                alert('Please complete all required fields in step ' + (i + 1));
                break;
            }
        }

        if (!isValid) {
            e.preventDefault(); // Prevent form submission only if validation fails
        }
        // If valid, allow the form to submit naturally
    });

    // Initialize the form by showing the first step
    showStep(0);

    // Add event handlers to existing remove buttons
    document.querySelectorAll(".remove-address").forEach((button) => {
        button.addEventListener("click", function () {
            if (document.querySelectorAll(".address-form").length > 1) {
                this.closest(".address-form").remove();
            }
        });
    });

    document.querySelectorAll(".remove-contact").forEach((button) => {
        button.addEventListener("click", function () {
            if (document.querySelectorAll(".contact-form").length > 1) {
                this.closest(".contact-form").remove();
            }
        });
    });
});
