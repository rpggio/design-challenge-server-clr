Feature: Arrivals
  In order to handle incoming flights,
  the system should schedule arrivals at gates.

  Background:
    Given the system is reset

  Scenario: Arrivals are scheduled at gates
    Given the client adds gates:
      | gate |
      | A1   |
    And the client adds flights:
      | flight | arrives | departs |
      | 22     | 8:00    | 9:00    |
    When the client requests arrivals
    Then the response is:
      | flight | gate | arrives |
      | 22     | A1   | 8:00    |

  Scenario: Arrivals are sorted by arrival time
    Given the client adds gates:
      | gate |
      | A1   |
    And the client adds flights:
      | flight | arrives | departs |
      | 1100   | 11:00   | 11:30   |
      | 190    | 9:00    | 9:30    |
    When the client requests arrivals
    Then the response is:
      | flight | gate | arrives |
      | 190    | A1   | 9:00    |
      | 1100   | A1   | 11:00   |

  Scenario: Arrivals are scheduled at gates as close to hub as possible
  # Gates with lower numbers are closer to the hub (A1 is closer than A2).
    Given the client adds gates:
      | gate |
      | A2   |
      | A1   |
    And the client adds flights:
      | flight | arrives | departs |
      | 33     | 8:00    | 8:30    |
      | 44     | 10:00   | 10:30   |
    When the client requests arrivals
    Then the response is:
      | flight | gate | arrives |
      | 33     | A1   | 8:00    |
      | 44     | A1   | 10:00   |
