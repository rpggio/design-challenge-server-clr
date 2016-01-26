Feature: Flights
  In order to support gate scheduling
  The system should be able to store flights

  One or more flights may be posted in a single request.
  Arrival time is formatted as h:mm.

  Background:
    Given the system is reset

  Scenario: Save flight arrival
    When the client adds flights:
      | flight | arrives |
      | 44     | 8:00    |
      | 55     | 10:00   |
    And the client requests flights
    Then the response is:
      | flight | arrives |
      | 44     | 8:00    |
      | 55     | 10:00   |
