# GitHub Action: Branch Details

[![build](https://github.com/StevenJDH/branch-details/actions/workflows/dotnet-action-sonar-container-workflow.yml/badge.svg?branch=main)](https://github.com/StevenJDH/branch-details/actions/workflows/dotnet-action-sonar-container-workflow.yml)
![GitHub release (latest by date including pre-releases)](https://img.shields.io/github/v/release/StevenJDH/branch-details?include_prereleases)
[![Public workflows that use this action.](https://img.shields.io/endpoint?style=flat&url=https%3A%2F%2Fused-by.vercel.app%2Fapi%2Fgithub-actions%2Fused-by%3Faction%3DStevenJDH%2Fbranch-details%26badge%3Dtrue)](https://github.com/search?o=desc&q=StevenJDH+branch-details+language%3AYAML&s=&type=Code)
![Maintenance](https://img.shields.io/badge/yes-4FCA21?label=maintained&style=flat)
![GitHub](https://img.shields.io/github/license/StevenJDH/branch-details)

Branch Details is a GitHub action that exposes information about branches, tags, and related concepts to simplify automation activities. It has been compiled using [Ahead-Of-Time (AOT)](https://en.wikipedia.org/wiki/Ahead-of-time_compilation) compilation to native code for increased performance and reduced memory usage. The native code is containerized using an Ubuntu-based [.NET Chiseled Container](https://devblogs.microsoft.com/dotnet/announcing-dotnet-chiseled-containers/) to further reduce the image size while significantly improving security and loading speeds. In fact, the base image is made up of only 6 files, which accounts for less than 10MB of the final image size. Do keep in mind that GitHub [only supports container actions on Linux runners](https://docs.github.com/en/actions/hosting-your-own-runners/managing-self-hosted-runners/about-self-hosted-runners#requirements-for-self-hosted-runner-machines), but as soon as this changes, support will be added. 

[![Buy me a coffee](https://img.shields.io/static/v1?label=Buy%20me%20a&message=coffee&color=important&style=flat&logo=buy-me-a-coffee&logoColor=white)](https://www.buymeacoffee.com/stevenjdh)

## Features

* Automatic processing of branch details in a predictable way to simplify workflows.
* Reference outputs for aggregated information associated with a branch.
* Optionally export outputs to environment variables.
* Summary reports are generated after each run.
* When testing locally, GitHub PATs can be auto loaded via [.NET's Secrets Manager](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-8.0&tabs=windows#secret-manager).

## Compatibility
Below is a list of GitHub-hosted runners that support jobs using this action.

| Runner     | Supported? | 
|------------|:----------:|
| [![Ubuntu](https://img.shields.io/badge/Ubuntu-E95420?style=flat&logo=ubuntu&logoColor=white)](https://docs.github.com/en/actions/reference/workflow-syntax-for-github-actions#jobsjob_idruns-on) | ✅ |
| [![Windows](https://img.shields.io/badge/Windows-0078D6?style=flat\&logo=windows\&logoColor=white)](https://docs.github.com/en/actions/reference/workflow-syntax-for-github-actions#jobsjob_idruns-on) | ❌ |
| [![macOS](https://img.shields.io/badge/macOS-000000?style=flat\&logo=macos\&logoColor=F0F0F0)](https://docs.github.com/en/actions/reference/workflow-syntax-for-github-actions#jobsjob_idruns-on) | ❌ |

## Inputs
The following inputs are available:

| Name                                                                         | Type     | Required | Default                         |  Description                                                        |
|------------------------------------------------------------------------------|----------|:--------:|:-------------------------------:|---------------------------------------------------------------------|
| <a name="drop-tag-prefix"></a>[drop&#x2011;tag&#x2011;prefix](#drop-tag-prefix) | `string` | `false`  | <code></code>                | Removes the prefix of a tag. For example, dropping the `v` in `v1.0.0` using `v`, or the more regex style of `[vV]`, as the input. |
| <a name="export-variables"></a>[export&#x2011;variables](#export-variables)  | `string` | `false`  | `false`                         | Indicates whether or not to set environment variables for each output exposed by this action for other steps in a job. Format is `BD_XXX` with all in uppercase. For example, `triggering_ref` becomes `BD_TRIGGERING_REF`. |
| <a name="github-token"></a>[github&#x2011;token](#github-token)              | `string` | `false`  | <code>&#xFEFF;$&#xFEFF;{{&#xa0;github.token&#xa0;}}</code> | Overrides the default GitHub token to authenticate API requests. |

## Outputs
The following outputs are available:

| Name                                                                 | Type     | Example(s) | Description                                                                         |
|----------------------------------------------------------------------|----------|------------|-------------------------------------------------------------------------------------|
| <a name="triggering_ref"></a>[triggering_ref](#triggering_ref)       | `string` | refs&#xFEFF;/&#xFEFF;heads&#xFEFF;/&#xFEFF;main, refs&#xFEFF;/&#xFEFF;pull&#xFEFF;/&#xFEFF;123&#xFEFF;/&#xFEFF;merge, refs&#xFEFF;/&#xFEFF;tags&#xFEFF;/&#xFEFF;v1.0.0 | The fully-formed ref of the branch or tag that triggered the workflow run. |
| <a name="is_tag"></a>[is_tag](#is_tag)                               | `string` | true       | Indicates whether or not `triggering_ref` is a tag.                                 |
| <a name="is_semver"></a>[is_semver](#is_semver)                      | `string` | true       | Indicates whether or not `tag` is using semantic versioning. Always false when `is_tag` is false. |
| <a name="tag"></a>[tag](#tag)                                        | `string` | v1.0.0, 1.0.0  | The tag that triggered the workflow run. Will be empty when `is_tag` is false.  |
| <a name="current_branch_name"></a>[current_branch_name](#current_branch_name) | `string`  | main, feature&#xFEFF;/&#xFEFF;example  | The current working branch regardless of event type. Usually same as `base_branch_name`, but for pull requests, it's same as `head_branch_name`. |
| <a name="is_default_branch"></a>[is_default_branch](#is_default_branch) | `string`  | true  | Indicates whether or not `current_branch_name` is the default branch.                |
| <a name="default_branch_name"></a>[default_branch_name](#default_branch_name) | `string`  | main  | The default branch for the repository.                                         |
| <a name="base_branch_name"></a>[base_branch_name](#base_branch_name) | `string` | main, develop  | The target branch of a commit, pull request, tag, or other event.               |
| <a name="head_branch_name"></a>[head_branch_name](#head_branch_name) | `string` | feature&#xFEFF;/&#xFEFF;example  | The source branch of a pull request, otherwise, it will be empty. |
| <a name="is_pull_request"></a>[is_pull_request](#is_pull_request)    | `string` | true       | Indicates whether or not `triggering_ref` is a pull request.                        |
| <a name="pull_request_id"></a>[pull_request_id](#pull_request_id)    | `string` | 123        | Id of the pull request. Will be empty when `is_pull_request` is false.              |
| <a name="repo_owner_name"></a>[repo_owner_name](#repo_owner_name)    | `string` | stevenjdh  | The username of the user who owns the repository.                                   |
| <a name="repo_name"></a>[repo_name](#repo_name)                      | `string` | my-repo    | The repository name.                                                                |
| <a name="event_actor_name"></a>[event_actor_name](#event_actor_name) | `string` | stevenjdh  | The username of the user or app that initiated the workflow run or rerun.           |
| <a name="is_event_actor_rerun"></a>[is_event_actor_rerun](#is_event_actor_rerun) | `string`  | false  | Indicates whether or not `event_actor` triggered the workflow rerun. May not always be the same actor for reruns, but uses original actor's privileges for it. Will always be false for first runs. |
| <a name="is_event_actor_owner"></a>[is_event_actor_owner](#is_event_actor_owner) | `string`  | true  | Indicates whether or not `event_actor` is the `repo_owner`.                 |

## Usage
Implementing this action is relatively simple with just a few steps. More practical examples can be found in the [Examples](Examples) folder.

```yaml
name: 'build'

on:
  push:
    branches:
    - main
  pull_request:
    branches:
    - main
    types: [opened, synchronize, reopened]

jobs:
  build:
    name: Build
    runs-on: ubuntu-latest

    steps:
    - name: Set Branch Outputs
      id: branch-details
      uses: stevenjdh/branch-details@v1
      with:
        drop-tag-prefix: 'v'

    - name: Display Action Outputs
      run: |
        echo "Action Outputs:"
        echo "- [triggering_ref]: ${{ steps.branch-details.outputs.triggering_ref }}"
        echo "- [is_tag]: ${{ steps.branch-details.outputs.is_tag }}"
        echo "- [is_semver]: ${{ steps.branch-details.outputs.is_semver }}"
        echo "- [tag]: ${{ steps.branch-details.outputs.tag }}"
        echo "- [current_branch_name]: ${{ steps.branch-details.outputs.current_branch_name }}"
        echo "- [is_default_branch]: ${{ steps.branch-details.outputs.is_default_branch }}"
        echo "- [default_branch_name]: ${{ steps.branch-details.outputs.default_branch_name }}"
        echo "- [base_branch_name]: ${{ steps.branch-details.outputs.base_branch_name }}"
        echo "- [head_branch_name]: ${{ steps.branch-details.outputs.head_branch_name }}"
        echo "- [is_pull_request]: ${{ steps.branch-details.outputs.is_pull_request }}"
        echo "- [pull_request_id]: ${{ steps.branch-details.outputs.pull_request_id }}"
        echo "- [repo_owner_name]: ${{ steps.branch-details.outputs.repo_owner_name }}"
        echo "- [repo_name]: ${{ steps.branch-details.outputs.repo_name }}"
        echo "- [event_actor_name]: ${{ steps.branch-details.outputs.event_actor_name }}"
        echo "- [is_event_actor_rerun]: ${{ steps.branch-details.outputs.is_event_actor_rerun }}"
        echo "- [is_event_actor_owner]: ${{ steps.branch-details.outputs.is_event_actor_owner }}"
```

## Disclaimer
Branch Details is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.

## Contributing
Thanks for your interest in contributing! There are many ways to contribute to this project. Get started [here](https://github.com/StevenJDH/.github/blob/main/docs/CONTRIBUTING.md).

## Do you have any questions?
Many commonly asked questions are answered in the FAQ:
[https://github.com/StevenJDH/branch-details/wiki/FAQ](https://github.com/StevenJDH/branch-details/wiki/FAQ)

## Want to show your support?

|Method          | Address                                                                                   |
|---------------:|:------------------------------------------------------------------------------------------|
|PayPal:         | [https://www.paypal.me/stevenjdh](https://www.paypal.me/stevenjdh "Steven's Paypal Page") |
|Cryptocurrency: | [Supported options](https://github.com/StevenJDH/StevenJDH/wiki/Donate-Cryptocurrency)    |


// Steven Jenkins De Haro ("StevenJDH" on GitHub)