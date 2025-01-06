const fs = require('fs');
const path = require('path');

const outputDir = path.resolve(__dirname, 'public');

// Ensure the public directory exists
if (!fs.existsSync(outputDir)) {
    fs.mkdirSync(outputDir);
}

// Write an example index.html file
const htmlContent = `<html><head><title>My Site</title></head><body><h1>Hello, Vercel!</h1></body></html>`;
fs.writeFileSync(path.join(outputDir, 'index.html'), htmlContent, 'utf8');

console.log("Build complete. Files written to public/ directory.");