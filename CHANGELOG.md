# Changelog
All notable changes to this project will be documented in this file.

## [2.0.0] - 2021-05-05
### Added
- Methods Exists and ExistsAsync, to check for existence of an entity in the database.
- Methods Count and CountAsync (Replacing GetCount and GetCountAsync)
- Support for more key object types in Get, Delete and Exists methods.
### Removed
- Methods GetCount and GetCountAsync (Replaced by Count adn CountAsync)

## [1.0.1] - 2021-04-27
### Fixed
- Correctly disposal of GridReader.

## [1.0.0] - Project created - 2021-04-22