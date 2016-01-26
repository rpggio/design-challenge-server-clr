Feature: Limited space
  To increase the utilization of the airport,
  the system should efficiently schedule flights in limited space.

  Background:
    Given the system is reset

  Scenario: Flights are scheduled to minimize the number of unscheduled flights
    Given the client adds gates:
      | gate |
      | A1   |
      | A2   |
      | A3   |
    And the client adds flights:
      | flight | arrives | departs |
      | 50     | 8:00    | 9:00    |
      | 51     | 9:10    | 10:00   |
      | 52     | 9:20    | 10:00   |
    When the client requests arrivals
    Then the response is:
      | flight | arrives | gate |
      | 50     | 8:00    | A2   |
      | 51     | 9:10    | A1   |
      | 52     | 9:20    | A3   |

  Scenario: Client can retrieve unscheduled flights
    Given the client adds gates:
      | gate |
      | A1   |
    And the client adds flights:
      | flight | arrives | departs |
      | 40     | 8:00    | 9:00    |
      | 41     | 8:01    | 9:01    |
    When the client requests arrivals
    Then the response is:
      | flight | arrives | gate |
      | 40     | 8:00    | A1   |
      | 41     | 8:01    |      |
    When the client requests flights/unscheduled
    Then the response is:
      | flight | arrives | departs |
      | 41     | 8:01    | 9:01    |
