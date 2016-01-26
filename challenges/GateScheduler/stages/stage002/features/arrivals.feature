Feature: Arrivals
  In order to handle incoming flights,
  the system should schedule arrivals at gates.

  The arrivals endpoint provides information for display on an 'Arrivals' screen in the terminal. 

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
