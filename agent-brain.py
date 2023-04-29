import pygame
import random

# Set up the screen dimensions and cell size
width, height = 500, 500
cell_size = 20

# Initialize Pygame and set the screen size
pygame.init()
screen = pygame.display.set_mode((width, height))

# Define some colors for drawing the maze
BLACK = (0, 0, 0)
WHITE = (255, 255, 255)

# Define the Cell class for generating the maze
class Cell:
    def __init__(self, row, col):
        # Keep track of the cell's row and column position
        self.row = row
        self.col = col
        # Keep track of which walls are present (True) or removed (False)
        self.walls = [True, True, True, True]
        # Keep track of whether the cell has been visited during generation
        self.visited = False

    def draw(self):
        # Calculate the cell's pixel coordinates on the screen
        x, y = cell_size * self.col, cell_size * self.row
        # If the cell has been visited, draw it in white
        if self.visited:
            pygame.draw.rect(screen, WHITE, (x, y, cell_size, cell_size))
        # If a wall exists on the North side of the cell, draw it
        if self.walls[0]:
            pygame.draw.line(screen, BLACK, (x, y), (x + cell_size, y))
        # If a wall exists on the East side of the cell, draw it
        if self.walls[1]:
            pygame.draw.line(screen, BLACK, (x + cell_size, y), (x + cell_size, y + cell_size))
        # If a wall exists on the South side of the cell, draw it
        if self.walls[2]:
            pygame.draw.line(screen, BLACK, (x + cell_size, y + cell_size), (x, y + cell_size))
        # If a wall exists on the West side of the cell, draw it
        if self.walls[3]:
            pygame.draw.line(screen, BLACK, (x, y + cell_size), (x, y))

    def get_neighbors(self, cells):
        print(f"get_neighbors called with params: {cells}")
        # Get the neighboring cells to this cell
        neighbors = []
        if self.row > 0:
            neighbors.append(cells[(self.row - 1) * cols + self.col])
        if self.col < cols - 1:
            neighbors.append(cells[self.row * cols + (self.col + 1)])
        if self.row < rows - 1:
            neighbors.append(cells[(self.row + 1) * cols + self.col])
        if self.col > 0:
            neighbors.append(cells[self.row * cols + (self.col - 1)])
        # Remove any neighbors that have already been visited
        neighbors = [neighbor for neighbor in neighbors if not neighbor.visited]
        print(f"get_neighbors returning: {[neighbor for neighbor in neighbors]}")
        return neighbors

    def remove_walls(self, other):
        print(f"remove_walls called with params: {other}")
        # Determine the direction between this cell and the other cell
        dx = self.row - other.row
        dy = self.col - other.col
        # If the other cell is to the North, remove the North wall of this cell and the South wall of the other cell
        if dx == 1:
            self.walls[0] = False
            other.walls[2] = False
        # If the other cell is to the South, remove the South wall of this cell and the North wall of the other cell
        elif dx == -1:
            self.walls[2] = False
            other.walls[0] = False
        # If the other cell is to the East, remove the East wall of this cell and the West wall of the other cell
        if dy == 1:
            self.walls[1] = False
            other.walls[3] = False
        # If the other cell is to the West, remove the West wall of this cell and the East wall of the other cell
        elif dy == -1:
            self.walls[3] = False
            other.walls[1] = False

# Set up the grid dimensions for the maze
rows, cols = height // cell_size, width // cell_size
cells = [Cell(row, col) for row in range(rows) for col in range(cols)]
current_cell = cells[0]
stack = []

# Main game loop for generating and drawing the maze
running = True
while running:
    for event in pygame.event.get():
        if event.type == pygame.QUIT:
            running = False

    # Draw the current state of the maze
    screen.fill(BLACK)
    for cell in cells:
        cell.draw()
    pygame.display.update()

    # Generate the maze using depth-first search with backtracking
    current_cell.visited = True
    neighbors = current_cell.get_neighbors(cells)
    print(f"current_cell: ({current_cell.row}, {current_cell.col})")
    print(f"unvisited neighbors: {[neighbor for neighbor in neighbors]}")
    if neighbors:
        # Choose a random neighboring cell to move to
        next_cell = random.choice(neighbors)
        # Push the current cell onto the stack
        stack.append(current_cell)
        # Remove the wall between the current cell and the chosen cell
        current_cell.remove_walls(next_cell)
        # Move to the chosen cell
        current_cell = next_cell
        print(f"chosen cell: ({next_cell.row}, {next_cell.col})")
    elif stack:
        # If no unvisited neighboring cells, backtrack to the previous cell on the stack
        current_cell = stack.pop()
        print(f"no unvisited neighbors, backtracking to cell: ({current_cell.row}, {current_cell.col})")
    else:
        print("maze generation complete!")
        running = False

# Print out any variables that were created during the program
print(f"width: {width}")
print(f"height: {height}")
print(f"cell_size: {cell_size}")
print(f"BLACK: {BLACK}")
print(f"WHITE: {WHITE}")
print(f"rows: {rows}")
print(f"cols: {cols}")
print(f"cells: {[cell for cell in cells]}")
print(f"current_cell: {current_cell}")
print(f"stack: {[cell for cell in stack]}")

# pygame.quit()
