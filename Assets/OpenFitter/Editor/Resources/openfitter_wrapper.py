import os
import sys
import bpy

def main():
    # Arguments after '--' in blender CLI are passed here
    # Since bpy might have its own args, we need to parse carefully.
    # We only care about arguments after '--'
    if "--" in sys.argv:
        argv = sys.argv[sys.argv.index("--") + 1:]
    else:
        argv = []

    # Find --script-path and its value manually to preserve all other arguments
    script_path = None
    remaining_args = []
    i = 0
    while i < len(argv):
        if argv[i] == "--script-path" and i + 1 < len(argv):
            script_path = argv[i + 1]
            i += 2  # Skip both --script-path and its value
        else:
            remaining_args.append(argv[i])
            i += 1

    if not script_path:
        print("[OpenFitter Wrapper] ERROR: --script-path not provided.")
        return

    script_dir = os.path.dirname(script_path)

    # addons_dir is still passed via environment variable for now, as it's project-wide
    addons_dir = os.environ.get("OPENFITTER_ADDONS_DIR")

    # 1. Add the core script directory to sys.path
    if script_dir and script_dir not in sys.path:
        sys.path.append(script_dir)
        print(f"[OpenFitter Wrapper] Added Core Dir to sys.path: {script_dir}")

    # 2. Add the custom addons directory and enable the addon
    # The addon itself will add its deps/ to sys.path when enabled
    if addons_dir and os.path.exists(addons_dir):
        if addons_dir not in sys.path:
            sys.path.append(addons_dir)
            print(f"[OpenFitter Wrapper] Added Addons Dir to sys.path: {addons_dir}")

        # Enable the addon - it will handle its own dependency paths
        addon_name = "robust-weight-transfer"
        try:
            bpy.utils.refresh_script_paths()
            bpy.ops.preferences.addon_enable(module=addon_name)
            print(f"[OpenFitter Wrapper] Enabled addon: {addon_name}")
        except Exception as e:
            print(f"[OpenFitter Wrapper] ERROR: Could not enable addon {addon_name}: {e}")
            print(f"[OpenFitter Wrapper] The fitting process may fail without this addon.")

    # 4. Execute the target script
    print(f"[OpenFitter Wrapper] Executing: {script_path}")
    
    # Update sys.argv so the sub-script only sees its arguments
    sys.argv = [script_path] + remaining_args

    global_vars = {
        "__name__": "__main__",
        "__file__": script_path,
        "bpy": bpy,
    }
    
    with open(script_path, 'r', encoding='utf-8') as f:
        code = compile(f.read(), script_path, 'exec')
        exec(code, global_vars)

if __name__ == "__main__":
    main()
