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
      | flight | arrives | 
      | 22     | 8:00    |
    When the client requests arrivals
    Then the response is:
      | flight | gate | arrives |
      | 22     | A1   | 8:00    |

 Scenario: Arrivals are sorted by arrival time
   Given the client adds gates:
     | gate |
     | A1   |
   And the client adds flights:
     | flight | arrives | 
     | 1100   | 10:00   |
     | 190    | 9:00    |
   When the client requests arrivals
   Then the response is:
     | flight | gate | arrives | 
     | 190    | A1   | 9:00    |
     | 1100   | A1   | 10:00   |

