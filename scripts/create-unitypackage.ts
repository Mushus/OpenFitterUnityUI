import * as fs from 'fs';
import * as path from 'path';
import * as tar from 'tar';

const ASSETS_ROOT = 'Assets/OpenFitter';
const OUTPUT_NAME = 'OpenFitter.unitypackage';
const TEMP_DIR = '.unitypackage-temp';

async function createUnityPackage() {
    if (!fs.existsSync(ASSETS_ROOT)) {
        console.error(`Error: ${ASSETS_ROOT} not found.`);
        process.exit(1);
    }

    if (fs.existsSync(TEMP_DIR)) {
        fs.rmSync(TEMP_DIR, { recursive: true, force: true });
    }
    fs.mkdirSync(TEMP_DIR);

    const files = getAllFiles(ASSETS_ROOT);

    for (const file of files) {
        if (file.endsWith('.meta')) continue;

        const metaFile = `${file}.meta`;
        if (!fs.existsSync(metaFile)) {
            console.warn(`Warning: Meta file not found for ${file}. Skipping.`);
            continue;
        }

        const guid = extractGuid(metaFile);
        if (!guid) {
            console.warn(`Warning: GUID not found in ${metaFile}. Skipping.`);
            continue;
        }

        const guidDir = path.join(TEMP_DIR, guid);
        fs.mkdirSync(guidDir);

        // asset file
        if (fs.statSync(file).isFile()) {
            fs.copyFileSync(file, path.join(guidDir, 'asset'));
        }

        // asset.meta file
        fs.copyFileSync(metaFile, path.join(guidDir, 'asset.meta'));

        // pathname file
        fs.writeFileSync(path.join(guidDir, 'pathname'), file.replace(/\\/g, '/'));
    }

    console.log(`Compressing to ${OUTPUT_NAME}...`);
    await tar.create(
        {
            gzip: true,
            file: OUTPUT_NAME,
            cwd: TEMP_DIR,
        },
        fs.readdirSync(TEMP_DIR)
    );

    fs.rmSync(TEMP_DIR, { recursive: true, force: true });
    console.log('Done!');
}

function getAllFiles(dir: string): string[] {
    const results: string[] = [];
    const list = fs.readdirSync(dir);

    // Add the directory itself (with its meta)
    results.push(dir);

    for (const file of list) {
        const fullPath = path.join(dir, file);
        const stat = fs.statSync(fullPath);
        if (stat.isDirectory()) {
            results.push(...getAllFiles(fullPath));
        } else {
            results.push(fullPath);
        }
    }
    return results;
}

function extractGuid(metaPath: string): string | null {
    const content = fs.readFileSync(metaPath, 'utf8');
    const match = content.match(/guid:\s*([a-f0-9]+)/);
    return match ? match[1] : null;
}

createUnityPackage().catch(err => {
    console.error(err);
    process.exit(1);
});
