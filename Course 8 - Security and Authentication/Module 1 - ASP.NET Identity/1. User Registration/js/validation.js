document.addEventListener('DOMContentLoaded', function () {
    const registrationForm = document.getElementById('registrationForm');
    const loginForm = document.getElementById('loginForm');
    const forgotPasswordForm = document.getElementById('forgotPasswordForm');

    initTogglePasswordButtons();

    if (registrationForm) {
        initRegistrationValidation(registrationForm);
    }

    if (loginForm) {
        initLoginValidation(loginForm);
    }

    if (forgotPasswordForm) {
        initSimpleFormValidation(forgotPasswordForm);
    }
});

function initTogglePasswordButtons() {
    document.querySelectorAll('.toggle-password').forEach(button => {
        button.addEventListener('click', function () {
            const targetId = this.getAttribute('data-target');
            const input = document.getElementById(targetId);
            const icon = this.querySelector('i');

            if (input.type === 'password') {
                input.type = 'text';
                icon.classList.replace('bi-eye', 'bi-eye-slash');
            } else {
                input.type = 'password';
                icon.classList.replace('bi-eye-slash', 'bi-eye');
            }
        });
    });
}

function initRegistrationValidation(form) {
    const password = document.getElementById('password');
    const confirmPassword = document.getElementById('confirmPassword');

    password.addEventListener('input', () => validatePasswords(password, confirmPassword));
    confirmPassword.addEventListener('input', () => validatePasswords(password, confirmPassword));

    initFieldValidation(form);
    initFormSubmission(form, () => validatePasswords(password, confirmPassword));
}

function initLoginValidation(form) {
    initFieldValidation(form);
    initFormSubmission(form);
}

function initSimpleFormValidation(form) {
    initFieldValidation(form);
    initFormSubmission(form);
}

function validatePasswords(password, confirmPassword) {
    const inputGroup = confirmPassword.closest('.input-group');

    if (confirmPassword.value === '') {
        confirmPassword.classList.remove('is-valid', 'is-invalid');
        if (inputGroup) inputGroup.classList.remove('is-valid', 'is-invalid');
        return;
    }

    const feedback = document.getElementById('confirmPasswordFeedback');

    if (password.value === confirmPassword.value) {
        confirmPassword.classList.remove('is-invalid');
        confirmPassword.classList.add('is-valid');
        confirmPassword.setCustomValidity('');
        if (inputGroup) {
            inputGroup.classList.remove('is-invalid');
            inputGroup.classList.add('is-valid');
        }
    } else {
        confirmPassword.classList.remove('is-valid');
        confirmPassword.classList.add('is-invalid');
        confirmPassword.setCustomValidity('Passwords do not match');
        if (feedback) feedback.textContent = 'Passwords do not match.';
        if (inputGroup) {
            inputGroup.classList.remove('is-valid');
            inputGroup.classList.add('is-invalid');
        }
    }
}

function initFieldValidation(form) {
    form.querySelectorAll('.form-control').forEach(input => {
        input.addEventListener('blur', function () {
            validateField(this);
        });

        input.addEventListener('input', function () {
            if (this.classList.contains('is-invalid')) {
                validateField(this);
            }
        });
    });
}

function validateField(field) {
    if (field.id === 'confirmPassword') {
        const password = document.getElementById('password');
        validatePasswords(password, field);
        return;
    }

    const inputGroup = field.closest('.input-group');

    if (field.checkValidity()) {
        field.classList.remove('is-invalid');
        field.classList.add('is-valid');
        if (inputGroup) {
            inputGroup.classList.remove('is-invalid');
            inputGroup.classList.add('is-valid');
        }
    } else {
        field.classList.remove('is-valid');
        field.classList.add('is-invalid');
        if (inputGroup) {
            inputGroup.classList.remove('is-valid');
            inputGroup.classList.add('is-invalid');
        }
    }
}

function initFormSubmission(form, additionalValidation) {
    const submitBtn = form.querySelector('#btnSubmit');

    form.addEventListener('submit', function (event) {
        event.preventDefault();
        event.stopPropagation();

        if (additionalValidation) {
            additionalValidation();
        }

        let isValid = true;
        form.querySelectorAll('.form-control').forEach(input => {
            validateField(input);
            if (!input.checkValidity() || input.classList.contains('is-invalid')) {
                isValid = false;
            }
        });

        if (isValid) {
            submitBtn.classList.add('loading');
            submitBtn.disabled = true;

            setTimeout(() => {
                submitBtn.classList.remove('loading');
                submitBtn.disabled = false;

                const successModal = new bootstrap.Modal(document.getElementById('successModal'));
                successModal.show();

                form.reset();
                form.querySelectorAll('.form-control').forEach(input => {
                    input.classList.remove('is-valid', 'is-invalid');
                });
                form.querySelectorAll('.input-group').forEach(group => {
                    group.classList.remove('is-valid', 'is-invalid');
                });
            }, 1500);
        }

        form.classList.add('was-validated');
    });
}
