#!/bin/sh
echo -ne '\033c\033]0;Inu's Adventure\a'
base_path="$(dirname "$(realpath "$0")")"
"$base_path/InusAdventure.x86_64" "$@"
