import * as fs from 'fs';
import * as path from 'path';

// Configuration
const PROJECT_ROOT = process.cwd();
const OPEN_FITTER_ROOT = path.join(PROJECT_ROOT, 'Assets', 'OpenFitter');
const PO_DIR = path.join(OPEN_FITTER_ROOT, 'Editor', 'Localization');

// Regex patterns
const L10N_PATTERN = /L10n\.Tr\s*\(\s*"([^"]+)"\s*\)/g;
const PO_MSGID_PATTERN = /^msgid\s+"(.*)"$/;
const PO_MSGSTR_PATTERN = /^msgstr\s+"(.*)"$/;

interface VerificationResult {
    missingKeys: string[];
    unusedKeys: string[];
}

function scanCsFiles(dir: string, foundKeys: Set<string>) {
    const files = fs.readdirSync(dir);

    for (const file of files) {
        const fullPath = path.join(dir, file);
        const stat = fs.statSync(fullPath);

        if (stat.isDirectory()) {
            scanCsFiles(fullPath, foundKeys);
        } else if (file.endsWith('.cs')) {
            const content = fs.readFileSync(fullPath, 'utf-8');
            let match;
            while ((match = L10N_PATTERN.exec(content)) !== null) {
                foundKeys.add(match[1]);
            }
        }
    }
}

function parsePoFile(filePath: string): Set<string> {
    const definedKeys = new Set<string>();
    const content = fs.readFileSync(filePath, 'utf-8');
    const lines = content.split(/\r?\n/);

    let isHeader = true; // Skip the first entry (header)

    for (const line of lines) {
        const trimmed = line.trim();
        const match = trimmed.match(PO_MSGID_PATTERN);
        if (match) {
            const key = match[1];
            if (key === "") {
                // Empty msgid usually denotes header or specific meta entry
                isHeader = true;
            } else {
                isHeader = false;
                definedKeys.add(key);
            }
        }
    }
    return definedKeys;
}

function verifyL10n() {
    console.log('Starting L10n Verification...');
    console.log(`Scanning directory: ${OPEN_FITTER_ROOT}`);

    if (!fs.existsSync(OPEN_FITTER_ROOT)) {
        console.error(`Error: Directory not found: ${OPEN_FITTER_ROOT}`);
        process.exit(1);
    }

    // 1. Find used keys in .cs files
    const usedKeys = new Set<string>();
    scanCsFiles(OPEN_FITTER_ROOT, usedKeys);
    console.log(`Found ${usedKeys.size} distinct keys used in code.`);

    // 2. Find defined keys in .po files (e.g., ja.po)
    if (!fs.existsSync(PO_DIR)) {
        console.warn(`Warning: PO directory not found: ${PO_DIR}. Assuming no translations.`);
    }

    const poFiles = fs.existsSync(PO_DIR)
        ? fs.readdirSync(PO_DIR).filter(f => f.endsWith('.po'))
        : [];

    let hasErrors = false;

    if (poFiles.length === 0) {
        console.warn('No .po files found to verify against.');
    }

    for (const poFile of poFiles) {
        console.log(`\nVerifying ${poFile}...`);
        const poPath = path.join(PO_DIR, poFile);
        const definedKeys = parsePoFile(poPath);

        const missingKeys = [...usedKeys].filter(key => !definedKeys.has(key));
        const unusedKeys = [...definedKeys].filter(key => !usedKeys.has(key));

        if (missingKeys.length > 0) {
            console.error(`\x1b[31m[ERROR] Found ${missingKeys.length} missing keys in ${poFile}:\x1b[0m`);
            missingKeys.forEach(k => console.error(`  - "${k}"`));
            hasErrors = true;
        } else {
            console.log(`\x1b[32m[OK] No missing keys in ${poFile}.\x1b[0m`);
        }

        if (unusedKeys.length > 0) {
            console.warn(`\x1b[33m[WARNING] Found ${unusedKeys.length} unused keys in ${poFile}:\x1b[0m`);
            unusedKeys.forEach(k => console.warn(`  - "${k}"`));
        } else {
            console.log(`[OK] No unused keys in ${poFile}.`);
        }
    }

    if (hasErrors) {
        console.error('\n\x1b[31mL10n Verification Failed.\x1b[0m');
        process.exit(1);
    } else {
        console.log('\n\x1b[32mL10n Verification Passed.\x1b[0m');
    }
}

verifyL10n();
