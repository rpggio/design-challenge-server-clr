Feature: Flights
  In order to support gate scheduling
  The system should be able to store flights

  Background:
    Given the system is reset

  Scenario: Save flight arrival
    When the client adds flights:
      | flight | arrives | departs |
      | 44     | 8:00    | 12:00   |
      | 55     | 10:00   | 11:00   |
    And the client requests flights
    Then the response is:
      | flight | arrives | departs |
      | 44     | 8:00    | 12:00   |
      | 55     | 10:00   | 11:00   |
