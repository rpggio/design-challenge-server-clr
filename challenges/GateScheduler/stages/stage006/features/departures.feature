Feature: Departures
  In order to handle outbound flights,
  the system should schedule departures at gates.

  Background:
    Given the system is reset

  Scenario: Departures are sorted by departure time
    Given the client adds gates:
      | gate |
      | A1   |
    And the client adds flights:
      | flight | arrives | departs | 
      | 22     | 10:00   | 11:00   |
      | 33     | 8:00    | 9:00    |
    When the client requests departures
    Then the response is:
      | flight | gate | departs | 
      | 33     | A1   | 9:00    |
      | 22     | A1   | 11:00   |

  Scenario: A gate is exclusively reserved for a flight between arrival and departure
    Given the client adds gates:
      | gate |
      | A1   |
      | A2   |
    And the client adds flights:
      | flight | arrives | departs | 
      | 22     | 8:00    | 11:00   |
      | 33     | 8:30    | 9:30    |
    When the client requests departures
    Then the response is:
      | flight | gate | departs | 
      | 33     | A2   | 9:30    |
      | 22     | A1   | 11:00   |
