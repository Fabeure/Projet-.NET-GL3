

const themeToggleBtn = document.getElementById('theme-switcher');
const themeName = 'dark-theme';
const darkTheme = localStorage.getItem(themeName);

// Vérification du thème actuel
if (darkTheme === null) {
    localStorage.setItem(themeName, 'inactive');
} else if (darkTheme === 'active') {
    toggleTheme();
}

// Écouteur d'événement pour le changement de thème
themeToggleBtn.addEventListener('click', function () {
    toggleTheme();
});

// Fonction pour basculer entre les thèmes
function toggleTheme() {

    const isDarkTheme = document.body.classList.contains('dark-theme');
    const allElements = document.querySelectorAll('*'); // Sélectionnez tous les éléments du site

    if (isDarkTheme) {
        // Si c'est en mode sombre, basculer vers le mode clair pour chaque élément
        allElements.forEach(element => {
            element.classList.remove('dark-theme');
            // Autres modifications de style pour le mode clair si nécessaire
        });
        localStorage.setItem(themeName, 'inactive');
    } else {
        // Sinon, basculer vers le mode sombre pour chaque élément
        allElements.forEach(element => {
            element.classList.add('dark-theme');
            // Autres modifications de style pour le mode sombre si nécessaire
        });
        localStorage.setItem(themeName, 'active');
    }
}
