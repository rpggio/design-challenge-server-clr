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
      | 22     | 9:00    | 9:30    |
      | 33     | 8:00    | 8:30    |
    When the client requests departures
    Then the response is:
      | flight | gate | departs | 
      | 33     | A1   | 8:30    |
      | 22     | A1   | 9:30    |

  Scenario: A gate is exclusively reserved for a flight between arrival and departure
    Given the client adds gates:
      | gate |
      | A1   |
      | A2   |
    And the client adds flights:
      | flight | arrives | departs | 
      | 22     | 8:00    | 10:00   |
      | 33     | 7:00    | 11:00   |
    When the client requests departures
    Then the response is:
      | flight | gate | departs | 
      | 22     | A2   | 10:00   |
      | 33     | A1   | 11:00   |
