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

  Scenario: Arrivals are scheduled at gates as close to hub as possible
    # Gates with lower numbers are closer to the hub (A1 is closer than A2).
    Given the client adds gates:
      | gate |
      | A2   |
      | A1   |
    And the client adds flights:
      | flight | arrives | 
      | 33     | 8:00    |
      | 44     | 9:00    |
    When the client requests arrivals
    Then the response is:
      | flight | gate | arrives |
      | 33     | A1   | 8:00    |
      | 44     | A1   | 9:00    |

  Scenario: Arrivals at a gate are separated by at least one hour
  # Arrivals separated by at least one hour can use the same gate.
    Given the client adds gates:
      | gate |
      | A1   |
      | A2   |
    And the client adds flights:
      | flight | arrives | 
      | 140    | 4:00    |
      | 145    | 4:50    |
      | 150    | 5:00    |
    When the client requests arrivals
    Then the response is:
      | flight | gate | arrives | 
      | 140    | A1   | 4:00    |
      | 145    | A2   | 4:50    |
      | 150    | A1   | 5:00    |

