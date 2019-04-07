$ORIG_DIR = "$PWD"

$COMMIT_SHA = & git rev-parse HEAD
$MASTER_COMMIT_SHA = & git rev-parse master
if ($COMMIT_SHA -ne $MASTER_COMMIT_SHA) {
    throw "Can only update the GitHub repository's ``gh-pages`` branch from the ``master`` branch! Aborting."
}

if ($env:CI -and !$env:GITHUB_ACCESS_TOKEN) {
    throw "When running during CI, can only update the GitHub repository's ``gh-pages`` when ``GITHUB_ACCESS_TOKEN`` is set. Aborting."
}

$GIT_STATUS = & git status --porcelain
if (![string]::IsNullOrWhitespace($GIT_STATUS)) {
    throw "Working directory is not clean, refusing to update documentation. Aborting."
}

$SOURCE_DIR = "$PWD/docs"
$GH_PAGES_DIR = "$PWD/.gh-pages"
$REPO_URL = & git remote get-url origin

if ($env:CI) {
    echo "Installing DocFX..."
    choco install -y docfx
}

echo "Flushing working directories..."
if (Test-Path $SOURCE_DIR/obj) {Remove-Item -Recurse -Force $SOURCE_DIR/obj}
if (Test-Path $SOURCE_DIR/_site) {Remove-Item -Recurse -Force $SOURCE_DIR/_site}
if (Test-Path $GH_PAGES_DIR) {Remove-Item -Recurse -Force $GH_PAGES_DIR}

echo "Running DocFX on ``$SOURCE_DIR/docfx.json``..."
docfx $SOURCE_DIR/docfx.json

$GH_PAGES_BRANCH_EXISTS = & git ls-remote --heads $REPO_URL gh-pages
if (![string]::IsNullOrWhitespace($GH_PAGES_BRANCH_EXISTS)) {
    echo "Retrieving the current ``gh-pages`` branch from ${REPO_URL}..."
    git clone $REPO_URL --branch gh-pages $GH_PAGES_DIR
}
else {
    echo "``gh-pages`` branch does not exist at $REPO_URL, initializing one..."
    mkdir $GH_PAGES_DIR
    cd $GH_PAGES_DIR
    git init
    git remote add origin $REPO_URL
    git commit --allow-empty -m "Start with an empty branch"
    git branch -m "gh-pages"
    cd $ORIG_DIR
}

echo "Updating local directory contents and creating a new commit..."
cd $GH_PAGES_DIR
git rm -r *
cp -Recurse $SOURCE_DIR/_site/* .
git add . -A
git commit -m "Sync with ``master`` branch at $COMMIT_SHA"

echo "Pushing updated docs to the GitHub repository's ``gh-pages`` branch..."
if ($env:CI) {
    echo "Adding GitHub personal access token to Git's credential store..."
    git config --global credential.helper store
    Add-Content "$env:USERPROFILE\.git-credentials" "https://$($env:GITHUB_ACCESS_TOKEN):x-oauth-basic@github.com`n"
    git config user.name "AppVeyor CI"
    git config user.email ""
}
git push origin gh-pages

echo "Documentation updated."
cd $ORIG_DIR
