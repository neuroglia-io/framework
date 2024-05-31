const { execSync } = require('child_process');
const fs = require('fs');

// Function to check if a package is installed
function isPackageInstalled(packageName) {
    try {
        require.resolve(packageName);
        return true;
    } catch (err) {
        return false;
    }
}

// Function to install a package
function installPackage(packageName) {
    console.log(`Installing ${packageName}...`);
    execSync(`npm install ${packageName}`, { stdio: 'inherit' });
}

// List of required packages
const requiredPackages = ['axios'];

// Check and install missing packages
requiredPackages.forEach(packageName => {
    if (!isPackageInstalled(packageName)) {
        installPackage(packageName);
    }
});

// Now you can require the package and use it
const axios = require('axios');

// Your script logic
const args = process.argv.slice(2);
console.log(JSON.stringify(args));

axios.get('https://jsonplaceholder.typicode.com/todos/1')
    .then(response => {
        console.log('Data:', response.data);
    })
    .catch(error => {
        console.error('Error fetching data:', error);
    });