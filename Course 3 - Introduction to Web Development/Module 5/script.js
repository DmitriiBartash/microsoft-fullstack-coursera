// ===== Toggle navigation menu (burger menu with animation) =====
const menuToggle = document.getElementById('menu-toggle');
const navList = document.querySelector('nav ul');

menuToggle?.addEventListener('click', () => {
  navList.classList.toggle('open');
  menuToggle.classList.toggle('active');
});

// ===== Smooth scroll for internal links =====
document.querySelectorAll('a[href^="#"]').forEach(anchor => {
  anchor.addEventListener('click', function (e) {
    e.preventDefault();
    const target = document.querySelector(this.getAttribute('href'));

    if (navList.classList.contains('open')) {
      navList.classList.remove('open');
      menuToggle.classList.remove('active');

      setTimeout(() => {
        target?.scrollIntoView({ behavior: 'smooth' });
      }, 400); 
    } else {
      target?.scrollIntoView({ behavior: 'smooth' });
    }
  });
});

// ===== Filter projects by category =====
const filterButtons = document.querySelectorAll('#project-filters button');
const projectCards = document.querySelectorAll('.project-card');

filterButtons.forEach(button => {
  button.addEventListener('click', () => {
    const category = button.getAttribute('data-category');
    filterProjects(category);

    filterButtons.forEach(btn => btn.classList.remove('active'));
    button.classList.add('active');
  });
});

function filterProjects(category) {
  projectCards.forEach(card => {
    const match = category === 'all' || card.dataset.category === category;
    card.style.display = match ? 'block' : 'none';
  });
}

// ===== Lightbox effect for project images =====
const lightbox = document.getElementById('lightbox');
const lightboxImg = document.getElementById('lightbox-img');
const lightboxClose = document.getElementById('lightbox-close');

document.querySelectorAll('.project-card img').forEach(img => {
  img.addEventListener('click', () => {
    lightboxImg.src = img.src;
    lightbox.style.display = 'flex';
  });
});

lightboxClose.addEventListener('click', () => {
  lightbox.style.display = 'none';
});

lightbox.addEventListener('click', (e) => {
  if (e.target === lightbox) {
    lightbox.style.display = 'none';
  }
});

// ===== Contact form validation =====
const form = document.querySelector('.contact-form');

form?.addEventListener('submit', function (e) {
  e.preventDefault();

  const name = form.name;
  const email = form.email;
  const message = form.message;

  let valid = true;

  [name, email, message].forEach(input => {
    input.style.borderColor = '';
    if (!input.value.trim()) {
      input.style.borderColor = 'red';
      valid = false;
    }
  });

  if (valid) {
    alert('Message sent successfully!');
    form.reset();
    [name, email, message].forEach(input => input.style.borderColor = '');
  } else {
    alert('Please fill in all required fields.');
  }
});

// ===== Real-time input feedback =====
['name', 'email', 'message'].forEach(id => {
  const input = document.getElementById(id);
  input?.addEventListener('input', () => {
    input.style.borderColor = input.value.trim() ? 'green' : 'red';
  });
});

// ===== Reveal and open obfuscated email on click =====
document.querySelectorAll('.email-obfuscated').forEach(el => {
  el.addEventListener('click', () => {
    const reversed = el.textContent.split('').reverse().join('');
    window.location.href = 'mailto:' + reversed;
  });
});
