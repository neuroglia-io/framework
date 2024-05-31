import sys
import subprocess
import json
import importlib.util

# Function to check if a package is installed
def is_package_installed(package_name):
    spec = importlib.util.find_spec(package_name)
    return spec is not None

# Function to install a package
def install_package(package_name):
    print(f"Installing {package_name}...")
    subprocess.check_call([sys.executable, '-m', 'pip', 'install', package_name])

# List of required packages
required_packages = ['requests']

# Check and install missing packages
for package_name in required_packages:
    if not is_package_installed(package_name):
        install_package(package_name)

# Now you can import the package and use it
import requests

# Your script logic
args = sys.argv[1:]  # The first argument is the script name, so slice it off

# Print arguments as JSON string
print(json.dumps(args))

try:
    response = requests.get('https://jsonplaceholder.typicode.com/todos/1')
    response.raise_for_status()  # Raise an HTTPError if the HTTP request returned an unsuccessful status code
    print('Data:', response.json())
    sys.exit(0)  # Exit with code 0 on success
except requests.RequestException as error:
    print('Error fetching data:', error)
    sys.exit(1)  # Exit with code 1 on error
