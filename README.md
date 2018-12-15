# unity-n-tests

For a long time the only real reason to depend on `unity-n-core` was for the
test helpers; this breaks those parts out as a stand-alone package.

## Usage

See the tests in the `Editor/` folder for each class for usage examples.

## Install

From your unity project folder:

    npm init
    npm install unity-n-tests --save
    echo Assets/pkg-all >> .gitignore
    echo Assets/pkg-all.meta >> .gitignore

The package and all its dependencies will be installed in
your Assets/pkg-all folder.

## Development

Setup and run tests:

    npm install
    npm install ..
    cd test
    npm install

Remember that changes made to the test folder are not saved to the package
unless they are copied back into the source folder.

To reinstall the files from the src folder, run `npm install ..` again.

### Tests

All tests are wrapped in `#if ...` blocks to prevent test spam.

You can enable tests in: Player settings > Other Settings > Scripting Define Symbols

The test key for this package is: `N_TESTS`
